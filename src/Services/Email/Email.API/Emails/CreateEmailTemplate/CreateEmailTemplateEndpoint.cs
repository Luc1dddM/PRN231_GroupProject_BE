using Carter;
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
        app.MapPost("/emails", async (HttpContext httpContext, CreateEmailTemplateRequest request, ISender sender) =>
        {
            try
            {

                // Lấy UserId từ headers
                var userId = httpContext.Request.Headers["UserId"].ToString();


                // Chuyển UserId vào command
                var command = new CreateEmailTemplateCommand(
                    request.Name,
                    request.Description,
                    request.Subject,
                    request.Body,
                    request.Active,
                    request.Category,
                    userId // Gán CreatedBy
                );


                // Chuyển UserId vào command
                /*var command = request.Adapt<CreateEmailTemplateCommand>();
                command.CreatedBy = userId; */


              /*  var command = request.Adapt<CreateEmailTemplateCommand>();*/

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