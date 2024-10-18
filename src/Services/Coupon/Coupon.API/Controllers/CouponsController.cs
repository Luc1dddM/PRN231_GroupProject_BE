using Coupon.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Exceptions;
using Coupon.API.Models;
namespace Coupon.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;

        public CouponsController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

  /*      // GET: api/coupons
        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _couponRepository.GetAllCoupons();
            return Ok(coupons);
        }*/

        // GET: api/coupons
        [HttpGet]
        public async Task<IActionResult> GetAllCoupons([FromQuery] GetListCouponParamsDto parameters)
        {
            var coupons = await _couponRepository.GetAllCoupons(parameters);
            return Ok(coupons);
        }

        // POST: api/coupons
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] Models.Coupon coupon)
        {
            if (coupon == null)
            {
                throw new BadRequestException("Coupon data is null.");
            }

            var userId = HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnAuthorizeException("UserId header is missing.");
            }

            coupon.CreatedBy = userId;
            coupon.CreatedDate = DateTime.UtcNow;

            var createdCoupon = await _couponRepository.CreateCoupon(coupon, userId);
            return CreatedAtAction(nameof(GetAllCoupons), new { id = createdCoupon.Id }, createdCoupon);
        }


        // PUT: api/coupons/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] Models.Coupon coupon)
        {
            if (coupon == null || coupon.Id != id)
            {
                throw new BadRequestException("Invalid coupon data.");
            }

            var userId = HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnAuthorizeException("UserId header is missing.");
            }

            var updatedCoupon = await _couponRepository.UpdateCoupon(coupon, userId);
            if (updatedCoupon == null)
            {
                throw new NotFoundException($"Coupon with id {id} not found.");
            }

            return Ok(updatedCoupon);
        }
    }
}
