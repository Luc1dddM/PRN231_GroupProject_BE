using BuildingBlocks.Models;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Categories.GetCategories
{
    public record GetCategoryRequest(GetListCategoryParamsDto getListCategoryParamsDto);

    public record GetCategoriesResponse(BaseResponse<PaginatedList<CategoryDTO>> Categories);

    public class GetCategoriesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/categories", async ( GetListCategoryParamsDto getListCategoryParamsDto, ISender sender) =>
            {

                var result = await sender.Send(new GetCategoriesQuery(getListCategoryParamsDto));

                var response = new GetCategoriesResponse(result.Categories);

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