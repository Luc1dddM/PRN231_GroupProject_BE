using Identity.Application.Email.Interfaces;
using Identity.Application.Identity.Dtos;
using Identity.Application.Identity.Interfaces;
using Identity.Application.RolePermission.Interfaces;
using Identity.Application.Utils;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Identity.Configuration;
using Identity.Infrastructure.Identity.Utils;
using IdentityModel;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Identity.Infrastructure.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelperFactory _urlHelper;
        private readonly UserManager<Domain.Entities.User> _userManager;
        private readonly Jwt _jwt;
        private readonly Configuration.RefreshToken _refresh;

        private readonly IRolePermissionService _permissionService;

        public AuthService(
            ApplicationDbContext context,
            IGoogleAuthService googleAuthService,
            IFacebookAuthService facebookAuthService,
            IEmailSender emailSender,
            IUrlHelperFactory urlHelperFactory,
            UserManager<Domain.Entities.User> userManager,
            IOptions<Jwt> jwt,
            IOptions<Configuration.RefreshToken> refresh,
            IRolePermissionService permissionService)
        {
            _context = context;
            _googleAuthService = googleAuthService;
            _facebookAuthService = facebookAuthService;
            _userManager = userManager;
            _urlHelper = urlHelperFactory;
            _jwt = jwt.Value;
            _emailSender = emailSender;
            _permissionService = permissionService;
            _refresh = refresh.Value;
        }

        /// <summary>
        /// Google SignIn 
        /// </summary>
        /// <param name="model">the view model</param>
        /// <returns>Task&lt;BaseResponse&lt;JwtResponseVM&gt;&gt;</returns>
        public async Task<BaseResponse<JwtModelVM>> SignInWithGoogle(string idToken)
        {

            var response = await _googleAuthService.GoogleSignIn(idToken);

            if (response.Errors.Any())
                return new BaseResponse<JwtModelVM>(response.ResponseMessage, response.Errors);

            var jwtResponse = await CreateJwtToken(response.Data);

            return new BaseResponse<JwtModelVM>(jwtResponse);
        }

        public async Task<BaseResponse< JwtModelVM>> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, password);

            if (user == null || isValid == false)
            {
                return new BaseResponse<JwtModelVM>(null, new List<string>() { "User name or password is not correct!" });
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new BaseResponse<JwtModelVM>(null, new List<string>() { "You have to confirm your account!" });
            }

            var jwtResponse = await CreateJwtToken(user);

            return new BaseResponse<JwtModelVM>(jwtResponse);

        }

        public async Task<BaseResponse<CreateCustomerDto>> Register(string email, string name, string phonenumber, string password)
        {
            Domain.Entities.User user = new()
            {
                FullName = name,
                UserName = email,
                Email = email,
                PhoneNumber = phonenumber,
                NormalizedEmail = email.ToUpper(),
            };

            try
            {
                user.Id = Guid.NewGuid().ToString();
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var result1 = await _userManager.AddToRoleAsync(user, "Admin");
                    SendConfirmationEmail(user.Email, user);
                    var userToResponse = _context.Users.First(u => u.UserName == email);
                    CreateCustomerDto userDto = new()
                    {
                        email = userToResponse.Email,
                        ID = userToResponse.Id,
                        name = userToResponse.FullName,
                        phonenumber = userToResponse.PhoneNumber
                    };
                    return new BaseResponse<CreateCustomerDto>(userDto);
                }
                return new BaseResponse<CreateCustomerDto>(null, new List<string>() { "Cannot Create User!" });
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateCustomerDto>(null, new List<string>() { ex.Message });
            }
        }

        public async Task<BaseResponse<string>> ReConfirmEmail(string emailAddress)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailAddress);
                if (user == null)
                {
                    return new BaseResponse<string>(null, new List<string>() { "User not found" });
                }
                await SendConfirmationEmail(emailAddress, user);
                return new BaseResponse<string>(null, "Thank you for confirming your email");
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>(null, new List<string>() { ex.Message });

            }
        }

        public async Task<BaseResponse<string>> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (userId == null || token == null)
            {
                return new BaseResponse<string>(null, new List<string>() { "User not found" });

            }
            else if (user == null)
            {
                return new BaseResponse<string>(null, new List<string>() { "User not found" });

            }
            else
            {
                token = token.Replace(" ", "+");
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return new BaseResponse<string>(null, "Thank you for confirming your email");
                }
                else
                {
                    return new BaseResponse<string>(null, new List<string>() { "User not found" });
                }
            }
        }

        private async Task SendConfirmationEmail(string? email, Domain.Entities.User? user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://localhost:7183/api/Identity/ConfirmEmail?UserId={user.Id}&Token={token}";
            await _emailSender.SendEmailAsync(email, "Confirm Your Email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>;.", true);
        }

        /// <summary>
        /// Renew Access Token
        /// </summary>
        /// <param name="request">the Jwt model</param>
        /// <returns>Task.String</returns>
        public async Task<BaseResponse<JwtModelVM>> RenewToken(JwtModelVM request)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.ASCII.GetBytes(_jwt.Secret);
            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = _jwt.ValidIssuer,
                ValidAudience = _jwt.ValidAudience,
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            };
            try
            {
                //check 1: AccessToken valid format
                var tokenInVerification = jwtTokenHandler.ValidateToken(request.Token, tokenValidateParam, out var validatedToken);

                //check 2: Check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)//false
                    {
                        return new BaseResponse<JwtModelVM>(null, new List<string>() { "Invalid token" });
                    }
                }

                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return new BaseResponse<JwtModelVM>(null, new List<string>() { "Access token has not yet expired" });
                }

                //check 4: Check refreshtoken exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);
                if (storedToken == null)
                {
                    return new BaseResponse<JwtModelVM>(null, new List<string>() { "Refresh token does not exist" });
                }

                var maxSessionDuration = TimeSpan.FromDays(Int32.Parse(_refresh.MaxDurationInDay));
                if(DateTime.UtcNow - storedToken.IssueAt > maxSessionDuration)
                {
                    return new BaseResponse<JwtModelVM>(null, new List<string>() { "Session Expired" });
                }

                if (storedToken.IsRevoked)
                {
                    return new BaseResponse<JwtModelVM>(null, new List<string>() { "Refresh token is Revoked" });
                }

                //check 5: check refreshToken is expired?
                if (storedToken.ExpiredAt < DateTime.UtcNow)
                {
                    return new BaseResponse<JwtModelVM>(null, new List<string>() { "Refresh token has expired" });
                }

                //check 6: AccessToken id == JwtId in RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return new BaseResponse<JwtModelVM>(null, new List<string>() { "Token doesn't match" });
                }
                //create new token
                var user = await _userManager.FindByIdAsync(storedToken.UserId);
                var token = await CreateJwtToken(user);

                _context.Remove(storedToken);
                await _context.SaveChangesAsync();

               
                return new BaseResponse<JwtModelVM>(token);
            }
            catch (Exception ex)
            {
                return new BaseResponse<JwtModelVM>(null, new List<string>() { "Something went wrong!" });
            }
        }

        /// <summary>
        /// Creates JWT Token
        /// </summary>
        /// <param name="user">the user</param>
        /// <returns>Task&lt;JwtModelVM</returns>
        private async Task<JwtModelVM> CreateJwtToken(Domain.Entities.User user)
        {

            //Get jwt info from configuration
            var key = Encoding.ASCII.GetBytes(_jwt.Secret);
            var userClaims = await BuildUserClaims(user);

            //Config token
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.ValidIssuer,
                notBefore: DateTime.UtcNow,
                audience: _jwt.ValidAudience,
                expires: DateTime.UtcNow.AddSeconds(Convert.ToInt32(_jwt.DurationInMinutes)),
                claims: userClaims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature));

            //Write token
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var refreshToken = GenerateRefreshToken();

            //Save Refresh token to database
            var refreshTokenEntity = new Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                JwtId = jwtSecurityToken.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssueAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(Convert.ToInt32(_jwt.DurationInMinutes)),
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();


            return new JwtModelVM
            {
                Token = accessToken,
                RefreshToken = refreshToken,
            };
        }

        /// <summary>
        /// Generates Refresh token 
        /// </summary>
        /// <returns>string;</returns>
        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            };
        }

        /// <summary>
        /// Builds the UserClaims
        /// </summary>
        /// <param name="user">the User</param>
        /// <returns>List&lt;System.Security.Claims&gt;</returns>
        private async Task<List<Claim>> BuildUserClaims(Domain.Entities.User user)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            List<string> permissions = await _permissionService.GetPermissionsAsync(user.Id);
            foreach (var permission in permissions)
            {
                userClaims.Add(new(CustomClaims.Permissions, permission));
            }

            return userClaims;
        }

        
        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}
