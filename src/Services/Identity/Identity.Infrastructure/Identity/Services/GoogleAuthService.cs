using Identity.Application.Identity.Dtos;
using Identity.Application.Identity.Interfaces;
using Identity.Application.Utils;
using Identity.Domain.Enums;
using Identity.Infrastructure.Configuration;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Identity.Utils;
using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using CUser = Identity.Domain.Entities.User;

namespace Identity.Infrastructure.Identity.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly UserManager<CUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly GoogleAuthConfig _googleAuthConfig;
        private readonly ILog _logger;

        public GoogleAuthService(
            UserManager<CUser> userManager,
            ApplicationDbContext context,
            IOptions<GoogleAuthConfig> googleAuthConfig
            )
        {
            _userManager = userManager;
            _context = context;
            _googleAuthConfig = googleAuthConfig.Value;
            _logger = LogManager.GetLogger(typeof(GoogleAuthService));
        }

        /// <summary>
        /// Google SignIn
        /// </summary>
        /// <param name="model">the model</param>
        /// <returns>Task&lt;BaseResponse&lt;User&gt;&gt;</returns>
        public async Task<BaseResponse<CUser>> GoogleSignIn(string idToken)
        {

            Payload payload = new();

            try
            {
                payload = await ValidateAsync(idToken, new ValidationSettings
                {
                    Audience = new[] { _googleAuthConfig.ClientId }
                });

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new BaseResponse<CUser>(null, new List<string> { "Failed to get a response" });
            }

            //Check Account Exist
            var existUser = await _userManager.FindByEmailAsync(payload.Email);
            if (existUser is null) {

                var userToBeCreated = new CreateUserFromSocialLogin
                {
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Email = payload.Email,
                    ProfilePicture = payload.Picture,
                    LoginProviderSubject = payload.Subject,
                };

                var user = await _userManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Google);

                if (user is not null)
                    return new BaseResponse<CUser>(user);
                else
                    return new BaseResponse<CUser>(null, new List<string> { "Unable to link a Local User to a Provider" });
            }

            //CHECKS IF THE USER HAS NOT ALREADY BEEN LINKED TO AN IDENTITY PROVIDER
            var userGooleProvider = await _userManager.FindByLoginAsync(LoginProvider.Google.ToString(), payload.Subject);

            if (userGooleProvider is not null)
                return new BaseResponse<CUser>(userGooleProvider);
            else
                return new BaseResponse<CUser>(null, new List<string> { "Email Already Used, Please Login By Internal Account" });
        }
    }
}
