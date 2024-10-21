using BuildingBlocks.Models;
using Carter;
using Chat.API.Messages.TotalNotify;
using Chat.API.Model.DTO;
using MediatR;

namespace Chat.API.Messages.GetUnreadMessageNotify
{
    public record GetUnreadMessageNotifyResponse(BaseResponse<List<UnReadNotifyDTO>> Response);

    public class GetUnreadMessageNotifyEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/chat/notify",
               async (ISender sender) =>
               {
                   var result = await sender.Send(new GetUnreadMessageNotifyQuery());

                   var response = new GetUnreadMessageNotifyResult(result.result);

                   return Results.Ok(response);
               })
               .WithName("Get Notify")
               .Produces<GetUnreadMessageNotifyResponse>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status400BadRequest)
               .WithSummary("Get Notify")
               .WithDescription("Get Notify");
        }
    }
}
