using BuildingBlocks.Models;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Products.CreateProduct;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsRequest(GetListProductParamsDto getListProductParamsDto);
    public record GetProductsResponse(BaseResponse<PaginatedList<ProductDTO>> Products);

    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapGet("/products",
            async (GetListProductParamsDto request, ISender sender) =>
            {
                

                var result = await sender.Send(new GetProductsQuery(request));

                var response = new GetProductsResponse(result.Products);

                return Results.Ok(response);
            })
            .WithName("GetProducts")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
        }
    }
}