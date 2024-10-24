namespace Catalog.API.Models.DTO
{
    public class ExportListParamsDto
    {
        public string? keyword { get; set; }
        public string[]? statuses { get; set; }
        public string[]? types { get; set; }
        public string? sortBy { get; set; } = "Id";
        public string? sortOrder { get; set; } = "desc";


/*        // Implement the BindAsync method for query string binding
        public static ValueTask<ExportListParamsDto?> BindAsync(HttpContext context)
        {
            var query = context.Request.Query;

            // Create a new instance and populate it from the query parameters
            var result = new ExportListParamsDto
            {
                Keyword = query["Keyword"].ToString(),
                Type = query["Type"].ToArray(),
                Statuses = query["Statuses"].ToArray(),
                SortBy = query["SortBy"].ToString() ?? "createdAt",
                SortOrder = query["SortOrder"].ToString() ?? "desc"
            };

            return ValueTask.FromResult<ExportListParamsDto?>(result);
        }*/
    }
}
