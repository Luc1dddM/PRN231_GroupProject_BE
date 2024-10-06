using Coupon.Grpc.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Coupon.Grpc.Services
{
    public class CouponService
     (Prn231GroupProjectContext dbContext, ILogger<CouponService> logger)
    : CouponProtoService.CouponProtoServiceBase
    {
            public override async Task<CouponModel> GetCoupon(GetCouponRequest request, ServerCallContext context)
            {
                var coupon = await dbContext
             .Coupons
             .FirstOrDefaultAsync(x => x.CouponCode == request.CouponCode);

                if (coupon is null)
                {
                    coupon = new Models.Coupon
                    {
                        CouponId = Guid.NewGuid().ToString(),
                        CouponCode = "No Coupon",
                        DiscountAmount = 0,
                        Status = false,
                        CreatedBy = "System",
                        CreatedDate = DateTime.UtcNow
                    };
                }

                logger.LogInformation("Coupon is retrieved for CouponCode: {CouponCode}, DiscountAmount: {DiscountAmount}", coupon.CouponCode, coupon.DiscountAmount);

                var couponModel = coupon.Adapt<CouponModel>();

                return couponModel;
            }

        public override async Task<GetCouponsResponse> GetCoupons(Empty request, ServerCallContext context)
        {
         
            var coupons = await dbContext.Coupons.ToListAsync();

            var couponModels = coupons.Adapt<List<CouponModel>>();

            var response = new GetCouponsResponse
            {
                Coupons = { couponModels }
            };

            return response;
        }

        public override async Task<CouponModel> CreateCoupon(CreateCouponRequest request, ServerCallContext context)
        {

            var coupon = new Models.Coupon
            {
                CouponCode = request.CouponCode,
                DiscountAmount = request.DiscountAmount,
                Status = request.Status,
                MinAmount = request.MinAmount,
                MaxAmount = request.MaxAmount,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            dbContext.Coupons.Add(coupon);
            await dbContext.SaveChangesAsync();

            return coupon.Adapt<CouponModel>();
        }

        public override async Task<CouponModel> EditCoupon(EditCouponRequest request, ServerCallContext context)
        {

            var coupon = await dbContext.Coupons.FindAsync(request.Id);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Coupon not found."));
            }

            coupon.CouponCode = request.CouponCode;
            coupon.DiscountAmount = request.DiscountAmount;
            coupon.Status = request.Status;
            coupon.MinAmount = request.MinAmount;
            coupon.MaxAmount = request.MaxAmount;
            coupon.UpdatedBy = "System"; 
            coupon.UpdatedDate = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return coupon.Adapt<CouponModel>();
        }



    }
}
