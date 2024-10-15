using MassTransit;
using System.Reflection;

namespace Email.API
{
    public static class Extensions
    {
        public static IServiceCollection AddMessageBroker
            (this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
        {
            //implement rabbitMQ Masstransit configuration
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                {
                    config.AddConsumers(assembly);
                }

                config.UsingRabbitMq((context, cfg) =>
                {
                    // Configure RabbitMQ host
                    cfg.Host(configuration["MessageBroker:Host"], "/", host => {
                        // Default rabbitMq authentication
                        host.Username(configuration["MessageBroker:Username"] ?? "guest");
                        host.Password(configuration["MessageBroker:Password"] ?? "guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
                /*configurator.Host(new Uri(configuration["MessageBroker:Host"], "/", host =>
                {
                    host.Username(configuration["MessageBroker:UserName"]);
                    host.Password(configuration["MessageBroker:Password"]);
                });
                configurator.UseRawJsonSerializer(RawSerializerOptions.AddTransportHeaders | RawSerializerOptions.CopyHeaders);
                configurator.ConfigureEndpoints(context);
            });*/

            });
            return services;
        }
    }
}
