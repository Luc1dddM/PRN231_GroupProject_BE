using Carter;
using Email.API.Models;
using Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Email.API.SendMail;

public class GetEmailSendDetailEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sent-emails/{id}", async (string id, Prn231GroupProjectContext context) =>
        {
            try
            {
                // Tìm email gửi theo ID
                var sentEmail = await context.EmailSends.FirstOrDefaultAsync(e => e.EmailSendId == id);
                if (sentEmail == null)
                {
                    return Results.NotFound($"Email with ID {id} not found.");
                }

                // Trả về thông tin chi tiết email
                return Results.Ok(sentEmail);
            }
            catch (DbUpdateException dbEx)
            {
                return Results.Problem("Database update error: " + dbEx.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while fetching email details: " + ex.Message);
            }
        })
        .WithName("GetSentEmailDetail")
        .Produces<EmailSend>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithSummary("Get details of a sent email by ID");
    }
}
