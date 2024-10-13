using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Reflection;

namespace Identity.API
{
    public static class DependencyInjection
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:5173")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod(); 
                                  });
            });
            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddProblemDetails();
            return services;
        }


        public static WebApplication UseApiServices(this WebApplication webApplication)
        {
            webApplication.UseCors(MyAllowSpecificOrigins);
            webApplication.UseExceptionHandler();
            return webApplication;
        }
    }
}
