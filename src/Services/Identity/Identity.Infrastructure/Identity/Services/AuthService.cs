using Identity.Application.Email.Interfaces;
using Identity.Application.Identity.Dtos;
using Identity.Application.Identity.Interfaces;
using Identity.Application.RolePermission.Interfaces;
using Identity.Application.Utils;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Identity.Configuration;
using Identity.Infrastructure.Identity.Utils;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        private readonly IRolePermissionService _permissionService;

        public AuthService(
            ApplicationDbContext context,
            IGoogleAuthService googleAuthService,
            IFacebookAuthService facebookAuthService,
            IEmailSender emailSender,
            IUrlHelperFactory urlHelperFactory,
            UserManager<Domain.Entities.User> userManager,
            IOptions<Jwt> jwt,
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
        }

        /// <summary>
        /// Google SignIn 
        /// </summary>
        /// <param name="model">the view model</param>
        /// <returns>Task&lt;BaseResponse&lt;JwtResponseVM&gt;&gt;</returns>
        public async Task<BaseResponse<JwtResponseVM>> SignInWithGoogle(string idToken)
        {

            var response = await _googleAuthService.GoogleSignIn(idToken);

            if (response.Errors.Any())
                return new BaseResponse<JwtResponseVM>(response.ResponseMessage, response.Errors);

            var jwtResponse = await CreateJwtToken(response.Data);

            var data = new JwtResponseVM
            {
                Token = jwtResponse,
            };

            return new BaseResponse<JwtResponseVM>(data);
        }

        public async Task<BaseResponse<JwtResponseVM>> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, password);

            if (user == null || isValid == false)
            {
                return new BaseResponse<JwtResponseVM>(null, new List<string>() { "User name or password is not correct!" });
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new BaseResponse<JwtResponseVM>(null, new List<string>() { "You have to confirm your account!" });
            }

            var jwtResponse = await CreateJwtToken(user);

            var data = new JwtResponseVM
            {
                Token = jwtResponse,
            };

            return new BaseResponse<JwtResponseVM>(data);

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
        /// Creates JWT Token
        /// </summary>
        /// <param name="user">the user</param>
        /// <returns>System.String</returns>
        private async Task<string> CreateJwtToken(Domain.Entities.User user)
        {

            var key = Encoding.ASCII.GetBytes(_jwt.Secret);
            var userClaims = await BuildUserClaims(user);

            var signKey = new SymmetricSecurityKey(key);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.ValidIssuer,
                notBefore: DateTime.UtcNow,
                audience: _jwt.ValidAudience,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwt.DurationInMinutes)),
                claims: userClaims,
                signingCredentials: new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
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
            foreach(var permission in permissions) {
                userClaims.Add(new(CustomClaims.Permissions, permission));
            }

            return userClaims;
        }
    }
}
