using Carter;
using Email.API.DTOs;
using Email.API.Repository;
using Email.DTOs;

namespace Email.API.Emails.ExportEmailTemplate
{
    public class ExportEmailTemplateEndpoint : ICarterModule
    {
        public record ExportRequest(string[]? statusesParam, string[]? categoriesParam, string? searchterm, int? pageNumberParam, int? pageSizeParam);
        public record ExportResponse(byte[] data);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/ExportEmail", async (HttpContext httpContext, [AsParameters] ExportRequest request, IEmailRepository emailRepository) =>
            {
                var userId = httpContext.Request.Headers["UserId"].ToString();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("UserId is required.");
                }


                var exportedData = await emailRepository.ExportEmailFilter(request.statusesParam, request.categoriesParam, request.searchterm, request.pageNumberParam.Value, request.pageSizeParam.Value);

                // Trả về file Excel
                var result = Results.File(exportedData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmailTemplates.xlsx");
                return result;
            })
          .WithName("Export Email Template")
          .Produces(StatusCodes.Status200OK)
          .WithSummary("Export Emails")
          .WithDescription("Export Emails")
          .DisableAntiforgery();
        }
    }
}
