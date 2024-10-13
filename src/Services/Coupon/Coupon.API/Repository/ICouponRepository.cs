
namespace Coupon.API.Repository
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Models.Coupon>> GetAllCouponsAsync();
        Task<Models.Coupon> CreateCoupon(Models.Coupon coupon, string userId);
        Task<Models.Coupon?> UpdateCoupon(Models.Coupon coupon, string userId);
    }
}
