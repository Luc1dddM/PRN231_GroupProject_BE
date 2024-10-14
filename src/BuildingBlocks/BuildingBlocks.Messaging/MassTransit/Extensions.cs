using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.MassTransit
{
    //This class will contain extension methods for setting up the Masstransit with RabbitMQ.
    public static class Extensions
    {

       
        public static IServiceCollection AddMessageBroker
            (this IServiceCollection services,IConfiguration configuration, Assembly? assembly = null)
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

                /* config.UsingRabbitMq((context, configurator) =>
                 {
                     configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                     {
                         host.Username(configuration["MessageBroker:UserName"]);
                         host.Password(configuration["MessageBroker:Password"]);
                     });
                     configurator.ConfigureEndpoints(context);
                 });*/
            });
            return services;
        }
    }
}
