using BuildingBlocks.Models;
using Catalog.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Categories.CreateCategory
{
    public record CreateCategoryRequest(string Name, string Type, bool Status);

    public record CreateCategoryResponse(BaseResponse<string> Id);

    public class CreateCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/categories",
            async (CreateCategoryRequest request, ISender sender) =>
                {

                    var command = request.Adapt<CreateCategoryCommand>();

                    var result = await sender.Send(command);

                    var response = result.Adapt<CreateCategoryResponse>();

                    return Results.Created($"/categories/{response.Id}", response);

                })
            .WithName("CreateCategory")
            .Produces<CreateCategoryResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Category")
            .WithDescription("Create Category");
        }
    }
}
