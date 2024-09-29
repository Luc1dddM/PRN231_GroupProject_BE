using BuildingBlocks.Behaviors;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Reflection;

namespace Identity.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
          
            return services;
        }


        public static WebApplication UseApiServices(this WebApplication webApplication)
        {
            return webApplication;
        }
    }
}
