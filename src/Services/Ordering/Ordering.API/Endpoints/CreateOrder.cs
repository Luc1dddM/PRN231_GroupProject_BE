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
    public record CreateOrderResponse(Guid Id);


    public class CreateOrder : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/orders", async (CreateOrderRequest request, ISender sender, HttpClient httpClient) =>
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

                var response = result.Adapt<CreateOrderResponse>();


                var url = $"https://localhost:7090/send-email-order?orderId={response.Id}&userEmail={Uri.EscapeDataString(customerEmail)}&couponCode={Uri.EscapeDataString(request.Order.CouponCode)}";

                await httpClient.PostAsync(url, null);

                return Results.Created($"/order/{response.Id}", response);
            })
            .WithName("CreateOrder")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Order")
            .WithDescription("Create Order");
        }
    }
}
