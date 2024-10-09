using Carter;
using Email.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Emails.GetEmailList;

public record GetEmailsRequest(string? Category = null, string? Name = null, string? Subject = null, bool? Status = null);
public record GetEmailsResponse(IEnumerable<EmailTemplate> Emails);

public class GetEmailTemplatesListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/emails", async ([AsParameters] GetEmailsRequest request, ISender sender) =>
        {

            var result = await sender.Send(new GetEmailsQuery(request.Category, request.Name, request.Subject, request.Status));
            var response = new GetEmailsResponse(result.Emails);

            return Results.Ok(response);
        })
        .WithName("GetEmails")
        .Produces<GetEmailsResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Emails")
        .WithDescription("Get Emails");
    }
}