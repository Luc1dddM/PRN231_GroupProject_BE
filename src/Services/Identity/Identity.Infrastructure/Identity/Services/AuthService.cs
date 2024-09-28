using Identity.Application.DTOs;
using Identity.Application.Email.Interfaces;
using Identity.Application.Identity.Interfaces;
using Identity.Application.Utils;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Identity.Configuration;
using Identity.Infrastructure.Identity.Utils;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelperFactory _urlHelper;
        private readonly UserManager<User> _userManager;
        private readonly Jwt _jwt;
        private readonly IPermissionService _permissionService;

        public AuthService(
            ApplicationDbContext context,
            IGoogleAuthService googleAuthService,
            IFacebookAuthService facebookAuthService,
            IEmailSender emailSender,
            IUrlHelperFactory urlHelperFactory,
            UserManager<User> userManager,
            IOptions<Jwt> jwt,
            IPermissionService permissionService)
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
        public async Task<BaseResponse<JwtResponseVM>> SignInWithGoogle(GoogleSignInVM model)
        {

            var response = await _googleAuthService.GoogleSignIn(model);

            if (response.Errors.Any())
                return new BaseResponse<JwtResponseVM>(response.ResponseMessage, response.Errors);

            var jwtResponse = await CreateJwtToken(response.Data);

            var data = new JwtResponseVM
            {
                Token = jwtResponse,
            };

            return new BaseResponse<JwtResponseVM>(data);
        }

        /// <summary>
        /// Facebook SignIn
        /// </summary>
        /// <param name="model">the view model</param>
        /// <returns>Task&lt;BaseResponse&lt;JwtResponseVM&gt;&gt;</returns>
        public async Task<BaseResponse<JwtResponseVM>> SignInWithFacebook(FacebookSignInVM model)
        {
            var validatedFbToken = await _facebookAuthService.ValidateFacebookToken(model.AccessToken);

            if (validatedFbToken.Errors.Any())
                return new BaseResponse<JwtResponseVM>(validatedFbToken.ResponseMessage, validatedFbToken.Errors);

            var userInfo = await _facebookAuthService.GetFacebookUserInformation(model.AccessToken);

            if (userInfo.Errors.Any())
                return new BaseResponse<JwtResponseVM>(null, userInfo.Errors);

            var userToBeCreated = new CreateUserFromSocialLogin
            {
                FirstName = userInfo.Data.FirstName,
                LastName = userInfo.Data.LastName,
                Email = userInfo.Data.Email,
                ProfilePicture = userInfo.Data.Picture.Data.Url.AbsoluteUri,
                LoginProviderSubject = userInfo.Data.Id,
            };

            var user = await _userManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Facebook);
            if (user is not null)
            {
                var jwtResponse = await CreateJwtToken(user);

                var data = new JwtResponseVM
                {
                    Token = jwtResponse,
                };
                return new BaseResponse<JwtResponseVM>(data);
            }

            return new BaseResponse<JwtResponseVM>(null, userInfo.Errors);

        }

        public async Task<BaseResponse<JwtResponseVM>> Login(LoginRequestDto loginRequest)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == loginRequest.userName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequest.password);

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

        public async Task<BaseResponse<UserDto>> Register(RegistrationRequestDto registrationRequest)
        {
            User user = new()
            {
                FullName = registrationRequest.name,
                UserName = registrationRequest.email,
                Email = registrationRequest.email,
                PhoneNumber = registrationRequest.phonenumber,
                NormalizedEmail = registrationRequest.email.ToUpper(),
            };

            try
            {
                user.Id = Guid.NewGuid().ToString();
                var result = await _userManager.CreateAsync(user, registrationRequest.password);
                if (result.Succeeded)
                {
                    var result1 = await _userManager.AddToRoleAsync(user, "Admin");
                    if (result1.Succeeded)
                    {
                        Console.WriteLine("Test");
                    }
                    SendConfirmationEmail(user.Email, user);
                    var userToResponse = _context.Users.First(u => u.UserName == registrationRequest.email);
                    UserDto userDto = new()
                    {
                        email = userToResponse.Email,
                        ID = userToResponse.Id,
                        name = userToResponse.FullName,
                        phonenumber = userToResponse.PhoneNumber
                    };
                    return new BaseResponse<UserDto>(userDto);
                }
                return new BaseResponse<UserDto>(null, new List<string>() { "Cannot Create User!" });
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserDto>(null, new List<string>() { ex.Message });
            }
        }

        public async Task<BaseResponse<string>> ReConfirmEmail(ReConfirmMailDto reConfirmMailDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(reConfirmMailDto.EmailAddress);
                if (user == null)
                {
                    return new BaseResponse<string>(null, new List<string>() { "User not found" });
                }
                await SendConfirmationEmail(reConfirmMailDto.EmailAddress, user);
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

        private async Task SendConfirmationEmail(string? email, User? user)
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
        private async Task<string> CreateJwtToken(User user)
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
        private async Task<List<Claim>> BuildUserClaims(User user)
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
