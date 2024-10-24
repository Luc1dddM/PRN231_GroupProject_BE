using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Carter;
using Email.API.DTOs;
using Email.API.Emails.GetEmailList;
using Email.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Email.API.Emails.ImportEmailTemplate;
public class ImportEmailTemplateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/ImportEmail", async (HttpContext httpContext, IFormFile excelFile, IEmailRepository emailRepository) =>
        {
            var userId = httpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("UserId is required.");
            }
            emailRepository.ImportEmailTemplate(excelFile,userId);

            return Results.Ok("Import Email Template success");
        })
        .WithName("Import Email Template")
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Import Emails")
        .WithDescription("Import Emails")
        .DisableAntiforgery();
    }
}
   

