using BuildingBlocks.Models;
using Catalog.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductRequest(ProductCreateDTO ProductCreateDTO);

    public record CreateProductResponse(BaseResponse<string> Id);

    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products",
                [IgnoreAntiforgeryToken]
            async ([FromForm] CreateProductRequest request, ISender sender) =>
                {


                    var result = await sender.Send(new CreateProductCommand(request.ProductCreateDTO));

                    var response = result.Adapt<CreateProductResponse>();

                    return Results.Created($"/products/{response.Id}", response);

                })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Accepts<CreateProductRequest>("multipart/form-data")
            .DisableAntiforgery()
            .WithSummary("Create Product")
            .WithDescription("Create Product");
        }
    }
}
