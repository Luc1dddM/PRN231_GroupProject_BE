using BuildingBlocks.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Products.GetProductById;

namespace Catalog.API.Products.GetProductDetail
{
    public record GetProductDetailForOrderResponse(BaseResponse<ProductDetailForOrderDTO> Product);

    public class GetProductDetailForOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/Order/{id}", async (string id, ISender sender) =>
            {
                var result = await sender.Send(new GetProductDetailForOrderQuery(id));

                var response = new GetProductDetailForOrderResponse(result.Product);

                return Results.Ok(response);
            })
            .WithName("GetProductByIdForOrder")
            .Produces<GetProductDetailForOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Id For Order")
            .WithDescription("Get Product By Id For Order");
        }
    }
}
