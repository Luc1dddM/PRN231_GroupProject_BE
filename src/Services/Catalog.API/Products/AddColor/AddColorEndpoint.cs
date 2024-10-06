using Catalog.API.Models.DTO;
using Catalog.API.Products.CreateProduct;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.AddColor
{
    public record AddColorRequest(string ColorId, string ProductId, int Quantity, bool Status);

    public record AddColorResponse(string Id);

    public class AddColorEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products/AddColor",
                [IgnoreAntiforgeryToken]
            async (AddColorRequest request, ISender sender) =>
                {

                    var command = request.Adapt<AddColorCommand>();
                    var result = await sender.Send(command);

                    var response = result.Adapt<AddColorResponse>();

                    return Results.Created($"/products/AddColor/{response.Id}", response);

                })
            .WithName("AddColor")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add Color")
            .WithDescription("Add Color");
        }
    }
}
