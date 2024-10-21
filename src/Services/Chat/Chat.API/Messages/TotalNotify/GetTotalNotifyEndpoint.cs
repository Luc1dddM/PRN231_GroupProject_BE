using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Carter;
using Chat.API.Groups.GetGroups;
using MediatR;

namespace Chat.API.Messages.TotalNotify
{

    public record GetTotalNotifyResponse(BaseResponse<int> Response);

    public class GetTotalNotifyEndpoint : ICarterModule
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public GetTotalNotifyEndpoint(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/chat/totalNotify",
                async (ISender sender) =>
                {
                    var userId = _contextAccessor.HttpContext.Request.Headers["UserId"].ToString();
                    if (string.IsNullOrEmpty(userId)) throw new BadRequestException("User Id Is Null");
                    var result = await sender.Send(new GetTotalNotifyQuery(userId));

                    var response = new GetTotalNotifyResponse(result.result);

                    return Results.Ok(response);
                })
                .WithName("Get Total Notify")
                .Produces<GetTotalNotifyResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Total Notify")
                .WithDescription("Get Total Notify");
        }
    }
}
