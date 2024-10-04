using Carter;
using Email.API.Models;
using Email.API.Repository;
using Email.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace Email.API.SendMail;

public class SendEmailTemplateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/send-email-template", async (HttpContext httpContext, EmailSend emailSend, ISenderEmail emailSender) =>
        {
            try
            {
                // Lấy UserId từ headers
                var userId = httpContext.Request.Headers["UserId"].ToString();
                emailSend.SenderId = userId; // Gán UserId cho SenderId


                // Gửi email theo template
                await emailSender.SendEmailByEmailTemplate(emailSend.Template, emailSend.Receiver);

                // Tạo bản ghi EmailSend
                emailSend.EmailSendId = Guid.NewGuid().ToString();
                emailSend.Sendate = DateTime.UtcNow;


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


        app.MapPost("/send-email-to-all", async (HttpContext httpContext, EmailSend emailSend, ISenderEmail emailSender) =>
        {
            try
            {
                // Lấy danh sách người dùng thông qua SenderEmail repository
                var users = await emailSender.GetUsersAsync();

                // Lấy UserId từ headers
                var userId = httpContext.Request.Headers["UserId"].ToString();
                emailSend.SenderId = userId;

                // Gửi email tới tất cả người dùng
                foreach (var user in users)
                {
                    await emailSender.SendEmailByEmailTemplate(emailSend.Template, user.Email);

                    // Tạo bản ghi EmailSend cho mỗi người dùng (nếu cần)
                    emailSend.EmailSendId = Guid.NewGuid().ToString();
                    emailSend.Receiver = user.Email;
                    emailSend.Sendate = DateTime.UtcNow;
                }

                return Results.Ok("Emails sent successfully to all users!");
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
             .WithName("SendEmailToAllUsers")
             .Produces(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .WithSummary("Send Email to All Users")
             .WithDescription("Sends an email using a template to all users in the system.");
    }
}



