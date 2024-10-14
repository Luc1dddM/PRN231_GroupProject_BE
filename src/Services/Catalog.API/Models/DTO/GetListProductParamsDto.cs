namespace Catalog.API.Models.DTO
{
    public class GetListProductParamsDto
    {
        public string? Keyword { get; set; }
        public string[]? colorParam { get; set; }
        public string[]? brand { get; set; }
        public string[]? device { get; set; }
        public string? Price1 { get; set; }
        public string? Price2 { get; set; }
        public bool? Statuse { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "createdAt";
        public string SortOrder { get; set; } = "desc";



        // Implement the BindAsync method for query string binding
        public static ValueTask<GetListProductParamsDto?> BindAsync(HttpContext context)
        {
            var query = context.Request.Query;

            // Create a new instance and populate it from the query parameters
            var result = new GetListProductParamsDto
            {
                Keyword = query["Keyword"].ToString(),
                colorParam = query["colorParam"].ToArray(),
                brand = query["brand"].ToArray(),
                device = query["device"].ToArray(),
                Price1 = query["Price1"].ToString(),
                Price2 = query["Price2"].ToString(),
                Statuse = bool.TryParse(query["Statuse"], out var statuse) ? statuse : (bool?)null,
                PageNumber = int.TryParse(query["PageNumber"], out var pageNumber) ? pageNumber : 1,
                PageSize = int.TryParse(query["PageSize"], out var pageSize) ? pageSize : 5,
                SortBy = query["SortBy"].ToString() ?? "createdAt",
                SortOrder = query["SortOrder"].ToString() ?? "desc"
            };

            return ValueTask.FromResult<GetListProductParamsDto?>(result);
        }
    }
}
