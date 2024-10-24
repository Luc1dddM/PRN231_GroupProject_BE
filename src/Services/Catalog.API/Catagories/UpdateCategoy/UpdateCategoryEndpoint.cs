using BuildingBlocks.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Products.CreateProduct;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Categories.UpdateCategory
{
    public record UpdateCategoryRequest(string Name, string Type, bool Status);
    public record UpdateCategoryResponse(BaseResponse<bool> IsSuccess);

    public class UpdateCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/Categories/{id}",
                [IgnoreAntiforgeryToken]
            async (string id,UpdateCategoryRequest request, ISender sender) =>
                {
                    var command = new UpdateCategoryCommand(id,request.Name,request.Type,request.Status);

                    var result = await sender.Send(command);

                    var response = result.Adapt<UpdateCategoryResponse>();

                    return Results.Ok(response);
                })
                .WithName("UpdateCategory")
                .Produces<UpdateCategoryResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("Update Category")
                .WithDescription("Update Category");
        }
    }
}
