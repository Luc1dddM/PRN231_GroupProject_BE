using Carter;
using Email.Models;
using FluentValidation;

namespace Email.API.SendMail;

public class SendEmailTemplateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/send-email-template", async (EmailSend emailSend, ISenderEmail emailSender) =>
        {
            try
            {
                // Gửi email theo template
                await emailSender.SendEmailByEmailTemplate(emailSend.Template, emailSend.Receiver);
                return Results.Ok("Email sent successfully using template!");
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
        .WithName("SendEmailTemplate")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Send Email using a Template")
        .WithDescription("Sends an email based on the provided email template and receiver.");
    }
}

