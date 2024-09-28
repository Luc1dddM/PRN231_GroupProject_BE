using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace Email.API.Emails.CreateEmailTemplate;

public record CreateEmailTemplateRequest(string Name, string Description, string Subject, string Body, bool Active, string Category);

public record CreateEmailTemplateResponse(int Id);

public class CreateEmailTemplateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/emails", async (CreateEmailTemplateRequest request, ISender sender) =>
        {
            try
            {
                var command = request.Adapt<CreateEmailTemplateCommand>();

                var result = await sender.Send(command);

                var response = new CreateEmailTemplateResponse(result.Id);

                return Results.Created($"/emails/{response.Id}", response);
            }
            catch (ValidationException ex)
            {
                return Results.Problem("Validation failed: " + ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return Results.Problem("An unexpected error occurred: " + ex.Message);
            }
        })
        .WithName("CreateEmailTemplate")
        .Produces<CreateEmailTemplateResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Email Template")
        .WithDescription("Create Email Template");
    }
}