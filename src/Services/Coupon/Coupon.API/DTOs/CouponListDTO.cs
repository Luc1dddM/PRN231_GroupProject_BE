using System.ComponentModel.DataAnnotations;

namespace Coupon.API.Models
{
    public class CouponListDTO
    {
        public int Id { get; set; }

        public string CouponId { get; set; } = Guid.NewGuid().ToString();
        public string CouponCode { get; set; } = null!;

        public double DiscountAmount { get; set; }

        public int Quantity { get; set; }

        public bool Status { get; set; }

        public double? MinAmount { get; set; }

        public double? MaxAmount { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
