

using Coupon.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Coupon.API.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly Prn231GroupProjectContext _dbContext;

        public CouponRepository(Prn231GroupProjectContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Models.Coupon> CreateCoupon(Models.Coupon coupon,string userId)
        {
            try
            {
                coupon.CreatedDate = DateTime.Now;
                coupon.CreatedBy = userId;
                _dbContext.Coupons.Add(coupon);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return coupon;
        }

        public async Task<IEnumerable<Models.Coupon>> GetAllCouponsAsync()
        {
            return await _dbContext.Coupons.ToListAsync();
        }


        public async Task<Models.Coupon?> UpdateCoupon(Models.Coupon coupon, string userId)
        {
            try
            {
                var existingCoupon = await _dbContext.Coupons.FindAsync(coupon.Id);

                if (existingCoupon == null)
                {
                    return null;
                }

                existingCoupon.CouponCode = coupon.CouponCode;
                existingCoupon.DiscountAmount = coupon.DiscountAmount;
                existingCoupon.Quantity = coupon.Quantity;
                existingCoupon.Status = coupon.Status;
                existingCoupon.MinAmount = coupon.MinAmount;
                existingCoupon.MaxAmount = coupon.MaxAmount;
                existingCoupon.UpdatedBy = userId;
                existingCoupon.UpdatedDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return existingCoupon;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the coupon.", ex);
            }
        }
    }
}
