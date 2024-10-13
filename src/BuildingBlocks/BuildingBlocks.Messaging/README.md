# BuildingBlocks.Messaging

## Get started

This project is an Class Library project serving as intermediary for synchronizing data between two or more services.

## Overview - RabbitMQ

- [ ] [RabbitMQ](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet)

Message Broker: Accept and Forward messages + Accept: move message into an exchange + Forward: forward message to a queue that services can subscribe

Producer/Consumer model (Publish/Subscribe)

Message are stored on queues (message buffer), so RabbitMQ also has persistence associated with it too
-> It's able to create new one if fail, then, recieve message stored inside message buffer

Exchange can be used for "routing" functionality
-> When message published, we send it to exchange and exchange has queue bound to it

## Overview - MassTransit

- [ ] [MassTransit](https://masstransit.io/documentation/configuration)

MassTransit fully supports RabbitMQ, including many of the advanced features and capabilities. This is going to abstract the need for us to make direct connection and use RabbitMQ API directly

## MassTransit Installation

Please install MassTransit package to **_"BuildingBlocks.Messaging"_** project (if it does not exist).

For .NET CLI:

```
dotnet add package MassTransit.RabbitMQ --version 8.2.5
```

For Package Manager:

```
NuGet\Install-Package MassTransit.RabbitMQ -Version 8.2.5
```

Then reference to this project to the service that you want to use

Add project reference .NET CLI:

```
cd [your_service]
dotnet add reference ../BuildingBlocks.Messaging
```

## How to use this project

1. Create folder name Events and MassTransit

```
BuildingBlocks.Messaging
    └ Events
    └ MassTransit
\
```

2. Create unique class that serves for synchronize data when [CREATE, UPDATE, REMOVE,...] to other service

```
Contracts
    └ Events
        └ IdentityCreatedEvent.cs
        └ IdentityDeletedEvent.cs
        └ IdentityUpdatedEvent.cs
        └ ...
```
3. Create class extentions (if dose not exist)

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

                config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]);
                        host.Password(configuration["MessageBroker:Password"]);
                    });
                    configurator.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}



FOR CONSUMER (UserManagement)

3.1. Create class as RabbitMQ Consumer related to those classes on BuildingBlocks.Messaging in your service project. For Example:

```
UserManagementAPI
    └ Consumers
        └ IdentityCreatedConsumer.cs
        └ IdentityDeletedConsumer.cs
        └ IdentityUpdatedConsumer.cs
        └ ...
```

3.2. Implement consumer handling (Subscribe message from Message Queue)

```
UserManagementAPI
    └ Consumers
        └ IdentityCreatedConsumer.cs


using AutoMapper;
using Contracts.IdentityManagement;
using Entities.Context;
using Entities.Models;
using MassTransit;

namespace UserManagementAPI.Consumers;

public class IdentityCreatedConsumer : IConsumer<IdentityCreated>
{
    private readonly IMapper _mapper;
    private readonly FamsContext _dbContext;

    public IdentityCreatedConsumer(IMapper mapper,
        FamsContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    //  Summary:
    //      Consume add new Identity from IdentityService then synchronized data
    public async Task Consume(ConsumeContext<IdentityCreated> context)
    {
        try
        {
            Console.WriteLine("--> Consuming Identity Created with email: {0}", context.Message.Email);

            // Map to Entity
            var user = _mapper.Map<User>(context.Message);
            // Add new Entity
            await _dbContext.Users.AddAsync(user);
            // Savechange DB
            await _dbContext.SaveChangesAsync();

            // End process...
            await Task.CompletedTask;

            Console.WriteLine("--> Complete consuming Identity Created...");
        }
        catch (Exception ex) when (ex is DbException)
        {
            // Handle error using EntityFramworkOutbox
            // Process retry message
        }
    }
}
```

3.3. Config to Program.cs

```
// Add Mass Transit
builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());
```

FOR PUBLISHER (Identity)

4.1. Implement publish message (Here - I want to publish message from Identity to UserManagement)

```
IdentityAPI
    └ Controllers
        └ AuthenticationController.cs

[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public AuthenticationController(
        IPublishEndpoint publishEndpoint,
        IMapper mapper
    )
    {
        _publishEndpoint = publishEndpoint;
        _mapper = mapper;
    }

    [HttpPost(APIRoutes.Identity.Register, Name = nameof(RegisterAsync))]
    public async Task<IActionResult> RegisterAsync([FromBody] IdentityRegisterRequest reqObj)
    {
        // Convert request obj to ApplicationUser
        var applicationUser = _mapper.Map<ApplicationUser>(reqObj);

        // Process check validation here... (if any)

        // Publish message to message broker for UserManagementAPI add new user data
        // Map model to intermidate model from Contracts
        var identityCreated = _mapper.Map<IdentityCreated>(applicationUser);

        // Publish to Message Broker (RabbitMQ Exchange)
        await _publishEndpoint.Publish(identityCreated);

        // Process response here...
    }

}
```

4.2. Config to Program.cs

```
// Add Mass Transit
builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());
```

## NOTE:

Please ensure that you declare RabbitMQ configuration within your appsettings.json or secret file as follows:

  "MessageBroker": {
    "Host": "amqp://localhost:5672",
    "Username": "guest",
    "Password": "guest"
  }

To use rabbitMQ you must run the rabbitMQ API. You can do it by installing rabbitMQ or use docker
For using Docker_Compose:
    In docker-compose.yml:

        messagebroker:
            image: rabbitmq:management 

    In docker-compose.override.yml:

        messagebroker:
            container_name: messagebroker
            hostname: ecommrece-mq
            environment: 
            - RABBITMQ_DEFAUT_USER=guest
            - RABBITMQ_DEFAUT_PASS=guest
            restart: always
            ports:
            - "5672:5672"
            - "15672:15672"

If you dont want to use docker-compose you can do this:
open command prompt => paste this:

docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management


