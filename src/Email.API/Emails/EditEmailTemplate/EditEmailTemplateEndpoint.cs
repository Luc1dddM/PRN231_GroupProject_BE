using BuildingBlocks.CQRS;
using Carter;
using Email.API.Models;
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
        app.MapPut("/emails", async (string id, EditEmailTemplateRequest request, ISender sender) =>
        {
            try
            {
                // Gắn id từ query string vào command
                var command = request.Adapt<EditEmailTemplateCommand>() with { EmailTemplateId = id };

                var result = await sender.Send(command);

                var response = new EditEmailTemplateResponse(result.IsSuccess);

                return Results.Ok(response);
            }
            catch (ValidationException ex)
            {
                return Results.Problem("Validation failed: " + ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.Problem("Not found: " + ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return Results.Problem("An unexpected error occurred: " + ex.Message);
            }
        })
        .WithName("EditEmailTemplate")
        .Produces<EditEmailTemplateResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Edit Email Template")
        .WithDescription("Edit an existing Email Template by ID from URL");
    }
}
