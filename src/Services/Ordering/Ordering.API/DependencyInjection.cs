using BuildingBlocks.Exceptions.Handler;

namespace Ordering.API
{
    public static class DependencyInjection
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddCarter();


            services.AddCors(opt =>
            {
                opt.AddPolicy(name: MyAllowSpecificOrigins,
                              policy =>
                              {
                                  policy.WithOrigins("http://localhost:5173")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                              });
            });


            //Add cross-cutting concern
            services.AddExceptionHandler<CustomExceptionHandler>();
            return services;
        }

        public static WebApplication UseApiServices(this WebApplication app)
        {
            app.MapCarter();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseExceptionHandler(opts => { });
            return app;
        }
    }
}
