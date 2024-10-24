using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Catalog.API.Categories.GetCategories;
using Catalog.API.Models.DTO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Data;


namespace Catalog.API.Catagories.ExportCategories
{

    public record ExportCategoriesRequest(ExportListParamsDto parameters);

    public record ExportCategoriesResponse(BaseResponse<DataTable> response);

    public class ExportCategoriesEndpoint:ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/Categories/ExportCategories", async (ExportCategoriesRequest parameters, ISender sender) =>
            {

                var result = await sender.Send(new ExportCategoriesQuery(parameters.parameters));

                var response = new ExportCategoriesResponse(result.response);
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.AddWorksheet(response.response.Result, "Categories Records");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        var file = Results.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "categories.xlsx");
                        return file;
                    }
                }

            })
            .WithName("ExportCategories")
            .Produces<ExportCategoriesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Export Categories")
            .WithDescription("Export Categories");
        }
    }
}
