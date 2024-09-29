using Identity.Application.Email.Interfaces;
using Identity.Application.Identity.Interfaces;
using Identity.Application.RolePermission.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Configuration;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Email.Configuration;
using Identity.Infrastructure.Email.Service;
using Identity.Infrastructure.FacebookAuthentication;
using Identity.Infrastructure.Identity.Configuration;
using Identity.Infrastructure.Identity.Handler;
using Identity.Infrastructure.Identity.Services;
using Identity.Infrastructure.Identity.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
         
            var connectionString = configuration.GetConnectionString("DefaultDb");

            //Add Dbcontext  
            services.AddDbContext<ApplicationDbContext>(options => options
            .UseSqlServer(connectionString));


            //Add Identity
            services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 3;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<GoogleAuthConfig>(configuration.GetSection("Google"));
            services.Configure<FacebookAuthConfig>(configuration.GetSection("Facebook"));

            services.AddHttpClient("Facebook", c =>
            {
                c.BaseAddress = new Uri(configuration.GetValue<string>("Facebook:BaseUrl"));
            });

            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IFacebookAuthService, FacebookAuthService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IRolePermissionService, RolePermissionSerivce>();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(5);
            });

            var jwtSection = configuration.GetSection("JWT");
            services.Configure<Jwt>(jwtSection);

            var appSettings = jwtSection.Get<Jwt>();
            var secret = Encoding.ASCII.GetBytes(appSettings.Secret);
            
            //Config Email settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddAuthorization();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            return services;
        }
    }
}
