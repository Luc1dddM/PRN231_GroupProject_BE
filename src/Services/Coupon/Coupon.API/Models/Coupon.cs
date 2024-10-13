using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coupon.API.Models
{
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CouponId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string CouponCode { get; set; } = null!;

        [Required]
        public double DiscountAmount { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public bool Status { get; set; }

        public double? MinAmount { get; set; }

        public double? MaxAmount { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
