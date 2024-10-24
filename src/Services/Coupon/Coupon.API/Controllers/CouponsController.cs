using Coupon.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Exceptions;
using Coupon.API.Models;
using BuildingBlocks.Models;
using ClosedXML.Excel;
using System.Composition;
using Coupon.API.DTOs;
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
        public async Task<BaseResponse<PaginatedList<CouponListDTO>>> GetAllCoupons([FromQuery] GetListCouponParamsDto parameters)
        {
            var coupons = await _couponRepository.GetAllCoupons(parameters);

            if (coupons == null || !coupons.Items.Any())
            {
                return new BaseResponse<PaginatedList<CouponListDTO>>("No coupons found.");
            }

            return new BaseResponse<PaginatedList<CouponListDTO>>(coupons, "Coupons retrieved successfully.");
        }



        // POST: api/coupons
        [HttpPost]
        public async Task<BaseResponse<Models.Coupon>> CreateCoupon([FromBody] Models.Coupon coupon)
        {
            if (coupon == null)
            {
                throw new BadRequestException("Coupon data is null.");
            }

            var userId = HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                throw new BadRequestException("UserId header is missing.");
            }

            coupon.CreatedBy = userId;
            coupon.CreatedDate = DateTime.UtcNow;

            var createdCoupon = await _couponRepository.CreateCoupon(coupon, userId);
            return new BaseResponse<Models.Coupon>(createdCoupon, "Coupon created successfully.");
        }



        // PUT: api/coupons/{id}
        [HttpPut("{id}")]
        public async Task<BaseResponse<Models.Coupon>> UpdateCoupon(int id, [FromBody] Models.Coupon coupon)
        {
            if (coupon == null || coupon.Id != id)
            {
                throw new BadRequestException("Invalid coupon data.");
            }

            var userId = HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                throw new BadRequestException("UserId header is missing.");
            }

            var updatedCoupon = await _couponRepository.UpdateCoupon(coupon, userId);
            if (updatedCoupon == null)
            {
                throw new NotFoundException($"Coupon with id {id} not found.");
            }

            return new BaseResponse<Models.Coupon>(updatedCoupon, "Coupon updated successfully.");
        }

        [HttpGet("{id}")]
        public async Task<BaseResponse<Models.Coupon>> GetCouponById(int id)
        {
            var coupon = await _couponRepository.GetCouponById(id);
            if (coupon == null)
            {
                throw new NotFoundException($"Coupon with id {id} not found.");
            }

            return new BaseResponse<Models.Coupon>(coupon, "Coupon retrieved successfully.");
        }

        [HttpPost("ImportCoupons")]
        [ProducesResponseType(typeof(BaseResponse<MemoryStream>), 200)]
        public async Task<ActionResult> ImportCoupons(IFormFile fileRequest, CancellationToken cancellation = default)
        {


            var userId = HttpContext.Request.Headers["UserId"].ToString();

            if (string.IsNullOrEmpty(userId))
                return BadRequest(new Exception("User Id Is Null"));

            var response = await _couponRepository.ImportCoupons(fileRequest, userId);

            if (response.Result != null)
            {
                return File(response.Result.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ErrorReport.xlsx");
            }

            return Ok(new { Message = "Coupons imported successfully." });
        }



        [HttpPost]
        [Route("ExportCoupons")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<ActionResult> ExportUsers([FromBody] ExportListCouponParamDto parameters)
        {
            var response = await _couponRepository.ExportCoupon(parameters);

            if (response == null || response.Result == null)
            {
                return NotFound(new BaseResponse<bool>("No users found to export."));
            }

            // Tạo workbook Excel
            using (XLWorkbook wb = new XLWorkbook())
            {
                // Thêm worksheet với dữ liệu người dùng
                wb.AddWorksheet(response.Result, "Coupon Records");

                using (MemoryStream ms = new MemoryStream())
                {
                    // Lưu workbook vào MemoryStream
                    wb.SaveAs(ms);

                    // Trả về file Excel
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "coupons.xlsx");
                }
            }
        }

    }
}
