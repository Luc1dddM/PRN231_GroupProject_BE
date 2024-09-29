using Carter;
using Email.Models;
using MediatR;

namespace Email.API.Emails.GetEmailTemplateById;

// phản hồi cho yêu cầu lấy email theo ID
public record GetEmailByIdResponse(EmailTemplate EmailTemplate);

public class GetEmailByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/emails/{id}", async (string id, ISender sender) =>
        {
            try
            {
                var result = await sender.Send(new GetEmailByIdQuery(id));

                var response = new GetEmailByIdResponse(result.EmailTemplate);

                return Results.Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                // Trả về 404 Not Found với thông báo lỗi nếu không tìm thấy email
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ không mong muốn khác
                return Results.Problem("An unexpected error occurred: " + ex.Message);
            }
        })
        .WithName("GetEmailById")
        .Produces<GetEmailByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Email By Id")
        .WithDescription("Get Email By Id");
    }
}
