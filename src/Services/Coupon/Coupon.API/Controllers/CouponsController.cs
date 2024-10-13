using Coupon.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/coupons
        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _couponRepository.GetAllCouponsAsync();
            return Ok(coupons);
        }

        // POST: api/coupons
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] Models.Coupon coupon)
        {
            if (coupon == null)
            {
                return BadRequest("Coupon data is null.");
            }

            var userId = HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId header is missing.");
            }
            coupon.CreatedBy = userId; 
            coupon.CreatedDate = DateTime.UtcNow;

            try
            {
                var createdCoupon = await _couponRepository.CreateCoupon(coupon, userId);
                return CreatedAtAction(nameof(GetAllCoupons), new { id = createdCoupon.Id }, createdCoupon);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // PUT: api/coupons/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] Models.Coupon coupon)
        {
            if (coupon == null || coupon.Id != id)
            {
                return BadRequest("Invalid coupon data.");
            }

            var userId = HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId header is missing.");
            }

            try
            {
                var updatedCoupon = await _couponRepository.UpdateCoupon(coupon, userId);
                if (updatedCoupon == null)
                {
                    return NotFound($"Coupon with id {id} not found.");
                }
                return Ok(updatedCoupon);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
