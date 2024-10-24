namespace Coupon.API.Models
{
    public class GetListCouponParamsDto
    {
        public string? Keyword { get; set; }
         public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public string[]? Statuse { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "createdAt";
        public string SortOrder { get; set; } = "desc";
    }
}
