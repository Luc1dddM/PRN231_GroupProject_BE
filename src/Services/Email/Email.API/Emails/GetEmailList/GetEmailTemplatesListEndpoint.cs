using Azure;
using BuildingBlocks.Models;
using Carter;
using Email.API.DTOs;
using Email.API.Repository;
using Email.DTOs;
using Email.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Emails.GetEmailList;

public record GetEmailsRequest(string? Category = null, string? Name = null, string? Subject = null, bool? Status = null);
public record GetEmailsResponse(EmailListDTO Emails);
public class GetEmailTemplatesListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/emails", async ([AsParameters] GetListEmailParamsDto request, IEmailRepository emailRepository) =>
        {
            var result = await emailRepository.GetList(request.Statuses, request.Categories, request.SearchTerm, request.SortBy, request.SortOrder, request.PageNumber, request.PageSize);

            var response = new BaseResponse<GetEmailsResponse>(new GetEmailsResponse(result));
            return Results.Ok(response);
        })
        .WithName("GetEmailsList")
        .Produces<GetEmailsResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Emails List")
        .WithDescription("Get Emails List");
    }
}