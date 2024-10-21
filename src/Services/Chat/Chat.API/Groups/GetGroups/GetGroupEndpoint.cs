using BuildingBlocks.Models;
using Carter;
using Chat.API.Messages.GetMessages;
using Chat.API.Model.DTO;
using MediatR;

namespace Chat.API.Groups.GetGroups
{
    public record GetGroupsResponse(BaseResponse<List<GroupDTO>> Response);


    public class GetGroupEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/chat/group",
                async (ISender sender) =>
                {
                    var result = await sender.Send(new GetGroupQuery());

                    var response = new GetGroupsResponse(result.result);

                    return Results.Ok(response);
                })
                .WithName("Get Groups")
                .Produces<GetGroupsResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Groups")
                .WithDescription("Get Groups");
        }
    }
}
