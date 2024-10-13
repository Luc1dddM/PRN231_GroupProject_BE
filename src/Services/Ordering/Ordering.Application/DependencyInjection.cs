using BuildingBlocks.Behaviors;
using BuildingBlocks.Messaging.MassTransit;
using Coupon.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ordering.Application
{
    //extensions method should required static classes
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {   
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });


            // Get gRPC Coupon service URL from configuration
            var couponServiceUrl = configuration["GrpcSettings:CouponServiceUrl"];

            services.AddGrpcClient<CouponProtoService.CouponProtoServiceClient>(option =>
            {
                option.Address = new Uri(couponServiceUrl!);
            });

            services.AddMessageBroker(configuration,Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
