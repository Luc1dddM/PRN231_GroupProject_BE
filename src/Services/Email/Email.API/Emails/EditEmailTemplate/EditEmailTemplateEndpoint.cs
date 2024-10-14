using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Carter;
using Email.API.Models;
using Email.API.Repository;
using Email.Models;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Emails.EditEmailTemplate;

public record EditEmailTemplateRequest(string EmailTemplateId, string Name, string Description, string Subject, string Body, bool Active, string Category);
public record EditEmailTemplateResponse(bool IsSuccess);

public class EditEmailTemplateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Lấy id từ query string
        app.MapPut("/emails", async (HttpContext httpContext, string id, EditEmailTemplateRequest request, IEmailRepository emailRepository) =>
        {
            var userId = httpContext.Request.Headers["UserId"].ToString();

            var emailTemplate = new EmailTemplate
            {
                EmailTemplateId = id,
                Name = request.Name,
                Description = request.Description,
                Subject = request.Subject,
                Body = request.Body,
                Active = request.Active,
                Category = request.Category,
                UpdatedBy = userId,
                UpdatedDate = DateTime.UtcNow
            };

            var result = await emailRepository.UpdateEmailTemplate(emailTemplate);
            return Results.Ok(new EditEmailTemplateResponse(true));
        })
           .WithName("EditEmailTemplate")
        .Produces<EditEmailTemplateResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Edit Email Template")
        .WithDescription("Edit an existing Email Template by ID from URL");
    }
}
