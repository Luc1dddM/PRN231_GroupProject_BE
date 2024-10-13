using Carter;
using Coupon.Grpc;
using Email.API.Models;
using Email.API.Repository;
using Email.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace Email.API.Emails.SendMail;

public class SendEmailTemplateEndpoint : ICarterModule
{
    private readonly CouponProtoService.CouponProtoServiceClient _couponServiceClient;
    public SendEmailTemplateEndpoint(CouponProtoService.CouponProtoServiceClient couponServiceClient)
    {
        _couponServiceClient = couponServiceClient;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/send-email-order", async (HttpContext httpContext, string orderId, string userEmail, string couponCode, IEmailRepository emailRepository) =>
        {
            try
            {
                var senderId = httpContext.Request.Headers["UserId"].ToString();


                await emailRepository.SendEmailOrder(orderId, userEmail, couponCode);

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
        .WithName("SendEmailOrder")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Send Email Order")
        .WithDescription("Sends an email based on the order.");


        app.MapPost("/send-email-template", async (HttpContext httpContext, string emailTemplateId, string userEmail, string? couponCode, IEmailRepository emailRepository) =>
        {
            try
            {
                var senderId = httpContext.Request.Headers["UserId"].ToString();

                var emailTemplate = await emailRepository.GetEmailTemplateById(emailTemplateId);

                if (emailTemplate == null)
                {
                    return Results.Problem($"Email template with ID '{emailTemplateId}' not found.", statusCode: StatusCodes.Status404NotFound);
                }

                if (!string.IsNullOrEmpty(couponCode))
                {
                    await emailRepository.SendEmailCoupon(emailTemplate, senderId, userEmail, couponCode);
                }
                else
                {
                    await emailRepository.SendEmailByEmailTemplate(emailTemplate, senderId, userEmail);
                }

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


        app.MapPost("/send-email-to-all", async (HttpContext httpContext, string emailTemplateId, string? couponCode, IEmailRepository emailRepository) =>
        {
            try
            {
                var senderId = httpContext.Request.Headers["UserId"].ToString();

                var emailTemplate = await emailRepository.GetEmailTemplateById(emailTemplateId);

                if (emailTemplate == null)
                {
                    return Results.Problem($"Email template with ID '{emailTemplateId}' not found.", statusCode: StatusCodes.Status404NotFound);
                }


                if (!string.IsNullOrEmpty(couponCode))
                {
                    await emailRepository.SendCouponToAll(emailTemplate, senderId, couponCode);
                }
                else
                {
                    await emailRepository.SendEmailToAll(emailTemplate, senderId);
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



