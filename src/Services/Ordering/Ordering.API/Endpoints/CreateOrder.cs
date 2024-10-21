using BuildingBlocks.Models;
using MassTransit;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.API.Endpoints
{
    //- Accepts a CreateOrderRequest object.
    //- Maps the request to a CreateOrderCommand.
    //- Uses MediatR to send the command to the corresponding handler.
    //- Returns a response with the created order's ID.

    public record CreateOrderRequest(OrderDtoRequest Order);
    public record CreateOrderResponse(BaseResponse<OrderDto> Response);


    public class CreateOrder : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/orders", async (CreateOrderRequest request, ISender sender, HttpClient httpClient, IPublishEndpoint publishEndpoint) =>
            {

                var shippingAddress = request.Order.ShippingAddress;
                var customerId = request.Order.CustomerId;
                string customerEmail;

                if (!string.IsNullOrEmpty(shippingAddress.EmailAddress))
                {
                    customerEmail = shippingAddress.EmailAddress;
                }
                else
                {
                    var userResponse = await httpClient.GetAsync($"https://localhost:7183/api/User/{customerId}");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        var user = await userResponse.Content.ReadFromJsonAsync<UserDto>();
                        if (user != null && !string.IsNullOrEmpty(user.Email))
                        {
                            customerEmail = user.Email;
                        }
                        else
                        {
                            throw new Exception("Unable to get email from user API.");
                        }
                    }
                    else
                    {
                        throw new Exception("User API call failed.");
                    }
                }


                var command = request.Adapt<CreateOrderCommand>();

                var result = await sender.Send(command);


                // Return 201 Created with the order URL and response
                var orderId = result.Result.Result.EntityId;
                var locationUri = $"/orders/{orderId}";

                //var url = $"https://localhost:7090/send-email-order?orderId={response.Id}&userEmail={Uri.EscapeDataString(customerEmail)}&couponCode={Uri.EscapeDataString(request.Order.CouponCode)}";

                //await httpClient.PostAsync(url, null);
                await publishEndpoint.Publish(new
                {
                    OrderId = orderId,
                    UserEmail = customerEmail,
                    CouponCode = request.Order.CouponCode
                });
                return Results.Created(locationUri, new CreateOrderResponse(result.Result));

            })
            .WithName("CreateOrder")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .WithSummary("Create Order")
            .WithDescription("Create Order");
        }
    }
}
