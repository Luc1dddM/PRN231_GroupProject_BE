
using BuildingBlocks.Models;
using Coupon.API.Models;

namespace Coupon.API.Repository
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Models.Coupon>> GetAllCoupons();
        Task<PaginatedList<CouponListDTO>> GetAllCoupons(GetListCouponParamsDto parameters);
        Task<Models.Coupon> CreateCoupon(Models.Coupon coupon, string userId);
        Task<Models.Coupon?> UpdateCoupon(Models.Coupon coupon, string userId);
    }
}
