using Coupon.Grpc.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Coupon.Grpc.Services
{
    public class CouponService : CouponProtoService.CouponProtoServiceBase
    {
        private readonly Prn231GroupProjectContext dbContext;
        private readonly ILogger<CouponService> logger;

        public CouponService(Prn231GroupProjectContext _dbContext, ILogger<CouponService> _logger)
        {
            dbContext = _dbContext;
            logger = _logger;
        }
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
                    Quantity = 0,
                    Status = false
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

    }
}
