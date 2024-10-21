using MassTransit;
using System.Reflection;

namespace Email.API
{
    public static class Extensions
    {
        //public static IServiceCollection AddMessageBroker
        //    (this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
        //{
        //    //implement rabbitMQ Masstransit configuration
        //    services.AddMassTransit(config =>
        //    {
        //        config.SetKebabCaseEndpointNameFormatter();

        //        if (assembly != null)
        //        {
        //            config.AddConsumers(assembly);
        //        }

        //        config.UsingRabbitMq((context, configurator) =>
        //        {
        //            configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
        //            {
        //                host.Username(configuration["MessageBroker:UserName"]);
        //                host.Password(configuration["MessageBroker:Password"]);
        //            });
        //            configurator.ConfigureEndpoints(context);
        //        });
        //    });
        //    return services;
        //}
    }
}
