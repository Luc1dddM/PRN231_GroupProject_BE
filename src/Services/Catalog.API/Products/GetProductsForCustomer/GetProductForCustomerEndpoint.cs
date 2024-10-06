using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Catalog.API.Products.GetProductsForCustomer;
using Catalog.API.Repository;

namespace Catalog.API.Products.GetProductsForCustomer
{
    public record GetProductsForCustomerResponse(IEnumerable<Product> Products);

    public class GetCategoriesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/productsForCustomer", async (ISender sender) =>
            {

                var result = await sender.Send(new GetProductsForCustomerQuery());

                var response = result.Adapt<GetProductsForCustomerResponse>();

                return Results.Ok(response);
            })
            .WithName("GetProductsForCustomer")
            .Produces<GetProductsForCustomerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products for Cutomer")
            .WithDescription("Get Products for Cutomer");
        }
    }
}
