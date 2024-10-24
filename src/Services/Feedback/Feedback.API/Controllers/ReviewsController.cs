using Catalog.API.Repository;
using Feedback.API.Models;
using Feedback.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Endpoints;
using Ordering.Domain.Enums;
using Ordering.Domain.ValueObjects;
using System.Text.Json;

namespace Feedback.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IProductRepository _productRepository;
        private readonly HttpClient _httpClient;

        public ReviewsController(IFeedbackRepository feedbackRepository, HttpClient httpClient)
        {
            _feedbackRepository = feedbackRepository;
            _httpClient = httpClient;
        }

        // Phương thức để lấy danh sách tất cả feedbacks
        [HttpGet("feedbackslist")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _feedbackRepository.GetFeedbackListAsync();
            return Ok(feedbacks);
        }

        [HttpPost("add-feedback")]
        public async Task<IActionResult> AddFeedback([FromBody] Review review)
        {
            if (review == null)
            {
                return BadRequest("Feedback cannot be null.");
            }

            // Kiểm tra xem người dùng đã hoàn thành đơn hàng với OrderId và ProductId trong review chưa
            var hasCompletedOrder = await HasUserCompletedOrderAsync(HttpContext, review.OrderId, review.ProductId);
            if (!hasCompletedOrder)
            {
                return BadRequest("You must complete an order for this product before leaving feedback.");
            }

            // Thêm feedback vào cơ sở dữ liệu
            var addedFeedback = await _feedbackRepository.AddFeedbackAsync(review);
            return CreatedAtAction(nameof(GetAllFeedbacks), new { id = addedFeedback.FeedbackId }, addedFeedback);
        }
        private async Task<bool> HasUserCompletedOrderAsync(HttpContext httpContext, string orderId, string productId)
        {
            // Lấy UserId từ headers và chuyển đổi sang Guid
            var userIdString = httpContext.Request.Headers["UserId"].ToString();
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new Exception("Invalid UserId format.");
            }

            // Gọi dịch vụ Order để lấy danh sách đơn hàng của người dùng
            var response = await _httpClient.GetAsync($"https://localhost:5053/orders/customer/{userId}");

            // Kiểm tra phản hồi từ dịch vụ
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var strResult = await response.Content.ReadAsStringAsync();
                var ordersResponse = JsonSerializer.Deserialize<GetOrdersByCustomerResponse>(strResult, options);

                // Kiểm tra xem người dùng đã hoàn thành đơn hàng với OrderId và ProductId này hay không
                //return ordersResponse.Response.Result.Any(order =>
                //    order.CustomerId == userId &&                         // Kiểm tra CustomerId
                //    order.EntityId == Guid.Parse(orderId) &&                   // Kiểm tra OrderId
                //    order.Status.Equals(OrderStatus.Completed) &&              // Kiểm tra Status
                //    order.OrderItems.Any(item => item.ProductId == Guid.Parse(productId))); // Kiểm tra ProductId
            }

            throw new Exception("Failed to fetch orders from the external service.");
        }


    }
}
