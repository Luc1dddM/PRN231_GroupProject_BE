using Identity.Application.Identity.Dtos;
using Identity.Domain.Enums;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using CUser = Identity.Domain.Entities.User;

namespace Identity.Infrastructure.Identity.Utils
{
    public static class CreateUserFromSocialLoginExtension
    {
        /// <summary>
        /// Creates user from social login
        /// </summary>
        /// <param name="userManager">the usermanager</param>
        /// <param name="context">the context</param>
        /// <param name="model">the model</param>
        /// <param name="loginProvider">the login provider</param>
        /// <returns>System.Threading.Tasks.Task&lt;User&gt;</returns>

        public static async Task<CUser> CreateUserFromSocialLogin(this UserManager<CUser> userManager, ApplicationDbContext context, CreateUserFromSocialLogin model, LoginProvider loginProvider)
        {
            var user = new CUser
            {
                FullName = model.LastName + " " + model.FirstName,
                Email = model.Email,
                UserName = model.Email,
                ProfilePicture = model.ProfilePicture
            };
            user.UserId = Guid.NewGuid().ToString();

            await userManager.CreateAsync(user);

            await userManager.AddToRoleAsync(user, "Admin");

            //EMAIL IS CONFIRMED; IT IS COMING FROM AN IDENTITY PROVIDER
            user.EmailConfirmed = true;

            await userManager.UpdateAsync(user);
            await context.SaveChangesAsync();

            UserLoginInfo userLoginInfo = null;
            switch (loginProvider)
            {
                case LoginProvider.Google:
                    {
                        userLoginInfo = new UserLoginInfo(loginProvider.ToString(), model.LoginProviderSubject, loginProvider.ToString().ToUpper());
                    }
                    break;
                case LoginProvider.Facebook:
                    {
                        userLoginInfo = new UserLoginInfo(loginProvider.ToString(), model.LoginProviderSubject, loginProvider.ToString().ToUpper());
                    }
                    break;
                default:
                    break;
            }

            //ADDS THE USER TO AN IDENTITY PROVIDER
            var result = await userManager.AddLoginAsync(user, userLoginInfo);

            if (result.Succeeded)
                return user;

            else
                return null;
        }
    }
}
