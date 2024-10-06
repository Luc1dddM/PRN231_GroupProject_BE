using Catalog.API.Models;

namespace Catalog.API.Categories.GetCategories
{

    public record GetCategoriesResponse(IEnumerable<Category> Categories);

    public class GetCategoriesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/categories", async (ISender sender) =>
            {

                var result = await sender.Send(new GetCategoriesQuery());

                var response = result.Adapt<GetCategoriesResponse>();

                return Results.Ok(response);
            })
            .WithName("GetCategories")
            .Produces<GetCategoriesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Categories")
            .WithDescription("Get Categories");
        }
    }
}