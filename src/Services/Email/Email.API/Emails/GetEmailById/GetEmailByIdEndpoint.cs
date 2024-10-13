using BuildingBlocks.Models;
using Carter;
using Email.API.Repository;
using Email.Models;
using MediatR;

namespace Email.API.Emails.GetEmailTemplateById;

// phản hồi cho yêu cầu lấy email theo ID
public record GetEmailByIdResponse(EmailTemplate EmailTemplate);

public class GetEmailByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/emails/{id}", async (string id, IEmailRepository emailRepository) =>
        {
            var result = await emailRepository.GetEmailTemplateById(id);
            var response = new GetEmailByIdResponse(result);

            return Results.Ok(new BaseResponse<GetEmailByIdResponse>(response));
        })
        .WithName("GetEmailById")
        .Produces<GetEmailByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Email By Id")
        .WithDescription("Get Email By Id");
    }
}
