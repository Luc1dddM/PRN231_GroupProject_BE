namespace Coupon.API.DTOs
{
    public class ExportListCouponParamDto
    {
        public string? Keyword { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public string[]? Statuse { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? SortBy { get; set; } = "Id";
        public string? SortOrder { get; set; } = "desc";
    }
}
