using Carter;
using Email.API.Repository;
using Email.Models;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Email.API.Emails.CreateEmailTemplate;

public record CreateEmailTemplateRequest(string Name, string Description, string Subject, string Body, bool Active, string Category);

public record CreateEmailTemplateResponse(int Id);

public class CreateEmailTemplateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/emails", async (HttpContext httpContext, CreateEmailTemplateRequest request, IEmailRepository emailRepository) =>
        {
            // Lấy UserId từ headers
            var userId = httpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("UserId is required.");
            }

            var emailTemplate = new EmailTemplate
            {
                EmailTemplateId = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                Subject = request.Subject,
                Body = request.Body,
                Active = request.Active,
                Category = request.Category,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userId
            };

            var newEmailTemplate = await emailRepository.AddEmailTemplate(emailTemplate);

            return Results.Created($"/emails/{newEmailTemplate.EmailTemplateId}",
                new CreateEmailTemplateResponse(int.Parse(newEmailTemplate.EmailTemplateId)));
        })
         .WithName("CreateEmailTemplate")
        .Produces<CreateEmailTemplateResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Create Email Template")
        .WithDescription("Create Email Template");
    }
}