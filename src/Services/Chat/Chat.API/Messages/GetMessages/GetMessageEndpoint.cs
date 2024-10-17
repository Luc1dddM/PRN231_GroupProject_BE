using BuildingBlocks.Models;
using Chat.API.Model.DTO;
using MediatR;
using Carter;

namespace Chat.API.Messages.GetMessages
{
    public record GetMessageResponse(BaseResponse<List<MessageDTO>> listMessage);

    public class GetMessageEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapGet("/chat/message/{groupId}",
            async (string groupId, ISender sender) =>
            {
                var result = await sender.Send(new GetMessageQuery(groupId));

                var response = new GetMessageResult(result.listMessage);

                return Results.Ok(response);
            })
            .WithName("Get Messages")
            .Produces<GetMessageResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Messages")
            .WithDescription("Get Messages");
        }
    }
}
