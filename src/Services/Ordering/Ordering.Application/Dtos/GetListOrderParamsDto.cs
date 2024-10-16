namespace Ordering.Application.Dtos
{
    public class GetListOrderParamsDto
    {
        public string? Keyword { get; set; }
        public string[]? Statuses { get; set; } //for filter
        public string[]? PaymentMethods { get; set; } //for filter
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "createdAt";
        public string SortOrder { get; set; } = "desc";
    }
}
