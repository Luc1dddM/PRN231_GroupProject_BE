using BuildingBlocks.Models;
using Catalog.API.Categories.GetCategories;
using Catalog.API.Models;

namespace Catalog.API.Catagories.GetColor
{
    public record GetColorsRequest(string productId);

    public record GetColorsResponse(BaseResponse<IEnumerable<Category>> Categories);

    public class GetColorsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/categories/color/{id}", async (string id, ISender sender) =>
            {
                var result = await sender.Send(new GetColorsQuery(id));

                var response = result.Adapt<GetColorsResponse>();

                return Results.Ok(response);
            })
            .WithName("GetColors")
            .Produces<GetColorsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Colors")
            .WithDescription("Get Colors");
        }
    }
}
