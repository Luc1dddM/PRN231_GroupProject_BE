using BuildingBlocks.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Products.CreateProduct;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductRequest(ProductUpdateDTO ProductUpdateDTO);
    public record UpdateProductResponse( BaseResponse<bool> IsSuccess);

    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products",
                [IgnoreAntiforgeryToken]
            async ([FromForm] UpdateProductRequest request, ISender sender) =>
                {


                    var result = await sender.Send(new UpdateProductCommand(request.ProductUpdateDTO));

                    var response = result.Adapt<UpdateProductResponse>();

                    return Results.Ok(response);
                })
                .WithName("UpdateProduct")
                .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Accepts<CreateProductRequest>("multipart/form-data")
                .DisableAntiforgery()
                .WithSummary("Update Product")
                .WithDescription("Update Product");
        }
    }
}
