using BuildingBlocks.Models;
using Catalog.API.Categories.GetCategoryById;
using Catalog.API.Models;
using Catalog.API.Models.DTO;

namespace Catalog.API.Categories.GetCategoryById
{
    public record GetCategoryByIdResponse(BaseResponse<Category> Category);

    public class GetCategoryByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/categories/{id}", async (string id, ISender sender) =>
            {
                var result = await sender.Send(new GetCategoryByIdQuery(id));

                var response = result.Adapt<GetCategoryByIdResponse>();

                return Results.Ok(response);
            })
            .WithName("GetCategoryById")
            .Produces<GetCategoryByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Category By Id")
            .WithDescription("Get Category By Id");
        }
    }
}
