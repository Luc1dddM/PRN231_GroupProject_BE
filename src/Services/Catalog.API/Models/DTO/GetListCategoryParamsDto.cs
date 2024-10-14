namespace Catalog.API.Models.DTO
{
    public class GetListCategoryParamsDto
    {
        public string? Keyword { get; set; }
        public string[]? Type { get; set; }
        public string[]? Statuses { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "createdAt";
        public string SortOrder { get; set; } = "desc";


        // Implement the BindAsync method for query string binding
        public static ValueTask<GetListCategoryParamsDto?> BindAsync(HttpContext context)
        {
            var query = context.Request.Query;

            // Create a new instance and populate it from the query parameters
            var result = new GetListCategoryParamsDto
            {
                Keyword = query["Keyword"].ToString(),
                Type = query["Type"].ToArray(),
                Statuses = query["Statuses"].ToArray(),
                PageNumber = int.TryParse(query["PageNumber"], out var pageNumber) ? pageNumber : 1,
                PageSize = int.TryParse(query["PageSize"], out var pageSize) ? pageSize : 5,
                SortBy = query["SortBy"].ToString() ?? "createdAt",
                SortOrder = query["SortOrder"].ToString() ?? "desc"
            };

            return ValueTask.FromResult<GetListCategoryParamsDto?>(result);
        }
    }


}
