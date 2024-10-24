using BuildingBlocks.Exceptions;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Models;
using Identity.Application.Email.Interfaces;
using Identity.Application.Identity.Dtos;
using Identity.Application.Identity.Interfaces;
using Identity.Application.RolePermission.Interfaces;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Identity.Configuration;
using Identity.Infrastructure.Identity.Utils;
using IdentityModel;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        private readonly IPublishEndpoint _publishEndpoint;

        public AuthService(
            ApplicationDbContext context,
            IGoogleAuthService googleAuthService,
            IFacebookAuthService facebookAuthService,
            IEmailSender emailSender,
            IUrlHelperFactory urlHelperFactory,
            UserManager<Domain.Entities.User> userManager,
            IOptions<Jwt> jwt,
            IOptions<Configuration.RefreshToken> refresh,
            IRolePermissionService permissionService,
            IPublishEndpoint publishEndpoint)
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
            _publishEndpoint = publishEndpoint;
        }

        /// <summary>
        /// Google SignIn 
        /// </summary>
        /// <param name="model">the view model</param>
        /// <returns>Task&lt;BaseResponse&lt;JwtResponseVM&gt;&gt;</returns>
        public async Task<BaseResponse<LoginReponseDto>> SignInWithGoogle(string idToken)
        {

            var userMessage = await _googleAuthService.GoogleSignIn(idToken);

            if (!userMessage.IsSuccess)
                return new BaseResponse<LoginReponseDto>(userMessage.Message);

            var user = userMessage.Result;
            var jwtResponse = await CreateJwtToken(user);
            var response = new LoginReponseDto()
            {
                Token = jwtResponse.Token,
                RefreshToken = jwtResponse.RefreshToken,
                Email = user.Email ?? "",
                UserId = user.UserId,
                UserType = "Customer"
            };


            return new BaseResponse<LoginReponseDto>(response);
        }

        public async Task<BaseResponse<LoginReponseDto>> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, password);

            if (user == null || isValid == false)
            {
                throw new UnAuthorizeException("User name or password is not correct!");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new BadRequestException("You have to confirm your account by email");
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Determine user type based on roles
            var userType = roles.Contains("Customer") ? "customer" : "admin";

            var jwtResponse = await CreateJwtToken(user);
            var response = new LoginReponseDto()
            {
                Token = jwtResponse.Token,
                RefreshToken = jwtResponse.RefreshToken,
                Email = user.Email ?? "",
                UserId = user.UserId,
                UserType = userType
            };

            return new BaseResponse<LoginReponseDto>(response);
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
                CreatedAt = DateTime.UtcNow,
            };

            user.UserId = Guid.NewGuid().ToString();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var result1 = await _userManager.AddToRoleAsync(user, "Customer");
                SendConfirmationEmail(user.Email, user);
                var userToResponse = _context.Users.First(u => u.UserName == email);
                CreateCustomerDto userDto = new()
                {
                    email = userToResponse.Email,
                    ID = userToResponse.UserId,
                    name = userToResponse.FullName,
                    phonenumber = userToResponse.PhoneNumber
                };
                var Event = new CreateUserEvent
                {
                    UserId = user.UserId,
                    Name = user.FullName,
                    IsCustomer = true,
                };
                _publishEndpoint.Publish(Event);

                return new BaseResponse<CreateCustomerDto>(userDto);
            }
            throw new Exception(result.Errors.ToString());
        }

        public async Task<BaseResponse<string>> ReConfirmEmail(string emailAddress)
        {
            var user = await _userManager.FindByEmailAsync(emailAddress);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            await SendConfirmationEmail(emailAddress, user);
            return new BaseResponse<string>("Thank you for confirming your email");
        }

        public async Task<BaseResponse<string>> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                throw new BadRequestException("UserId or Token not found! please try later!");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found");

            token = token.Replace(" ", "+");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return new BaseResponse<string>("Thank you for confirming your email");
            }
            throw new BadRequestException(result.Errors.First().ToString());
        }

        public async Task<BaseResponse<string>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) throw new NotFoundException("Account is not exist!");

            if (!await _userManager.IsEmailConfirmedAsync(user)) throw new BadRequestException("Please confirm your account!");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var resetLink = $"http://localhost:5173/ResetPassword?Email={email}&Code={code}";
            _emailSender.SendEmailAsync(email, "Click Here To Reset Password", $"Please confirm your account by <a href='{resetLink}'>clicking here</a>;.", true);
            return new BaseResponse<string>("Send Email Reset Password Successfully! Please Check Your Email");
        }

        public async Task<BaseResponse<string>> ForgotPasswordConfirm(string email, string code, string password)
        {
            var decode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new NotFoundException("User Not Found!");

            var result = await _userManager.ResetPasswordAsync(user, decode, password);
            if (result.Succeeded) return new BaseResponse<string>("Reset Password Successfully");

            throw new Exception(result.Errors.First().ToString());

        }

        private async Task SendConfirmationEmail(string? email, Domain.Entities.User? user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://localhost:8080/api/Identity/ConfirmEmail?UserId={user.Id}&Token={token}";
            _emailSender.SendEmailAsync(email, "Confirm Your Email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>;.", true);
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

            //check 1: AccessToken valid format
            var tokenInVerification = jwtTokenHandler.ValidateToken(request.Token, tokenValidateParam, out var validatedToken);

            //check 2: Check alg
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase);
                if (!result)//false
                    throw new BadRequestException("Invalid token");
            }

            //check 3: Check accessToken expire?
            var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
            if (expireDate > DateTime.UtcNow)
                throw new BadRequestException("Access token has not yet expired");


            //check 4: Check refreshtoken exist in DB
            var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);
            if (storedToken == null)
                throw new BadRequestException("Refresh token does not exist");


            var maxSessionDuration = TimeSpan.FromDays(Int32.Parse(_refresh.MaxDurationInDay));
            if (DateTime.UtcNow - storedToken.IssueAt > maxSessionDuration)
                throw new BadRequestException("Session Expired");


            if (storedToken.IsRevoked)
                throw new BadRequestException("Refresh token is Revoked");

            //check 5: check refreshToken is expired?
            if (storedToken.ExpiredAt < DateTime.UtcNow)
                throw new BadRequestException("Refresh token has expired");

            //check 6: AccessToken id == JwtId in RefreshToken
            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (storedToken.JwtId != jti)
                throw new BadRequestException("Token doesn't match");

            //create new token
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == storedToken.UserId);
            var token = await CreateJwtToken(user);

            return new BaseResponse<JwtModelVM>(token);

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
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_jwt.DurationInMinutes)),
                claims: userClaims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature));

            //Write token
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var refreshToken = GenerateRefreshToken();

            //Save Refresh token to database
            var refreshTokenEntity = new Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.UserId,
                JwtId = jwtSecurityToken.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssueAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(Convert.ToInt32(_jwt.DurationInMinutes)),
            };

            var cToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == user.UserId);
            if (cToken != null) _context.Remove(cToken);

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();


            return new JwtModelVM
            {
                Token = accessToken,
                RefreshToken = refreshToken
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
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Id, user.UserId),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            List<string> permissions = await _permissionService.GetPermissionsAsync(user.UserId);

            userClaims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
            userClaims.AddRange(permissions.Select(permission => new Claim(CustomClaims.Permissions, permission)));
            return userClaims;
        }


        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }

        public Task<BaseResponse<string>> ForgotPasswordConfirm(string email, string code)
        {
            throw new NotImplementedException();
        }
    }
}
