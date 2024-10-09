using Catalog.API.Repository;
using Feedback.API.Models;
using Feedback.API.Repository;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Endpoints;
using Ordering.Domain.Enums;
using System.Text.Json;

namespace Feedback.API.Controllers
{

    public class ReviewController : Controller
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IProductRepository _productRepository;
        private readonly HttpClient _httpClient;

        public ReviewController(IFeedbackRepository feedbackRepository, HttpClient httpClient)
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

        private async Task<bool> HasUserCompletedOrderAsync(HttpContext httpContext, string productId)
        {
            // Lấy UserId từ headers và chuyển đổi sang Guid
            var userIdString = httpContext.Request.Headers["UserId"].ToString();
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new Exception("Invalid UserId format.");
            }

            // Gọi dịch vụ Order để lấy danh sách đơn hàng của người dùng
            var response = await _httpClient.GetAsync($"https://localhost:5053/orders/customer/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var strResult = await response.Content.ReadAsStringAsync();
                var ordersResponse = JsonSerializer.Deserialize<GetOrdersByCustomerResponse>(strResult, options);

                // Kiểm tra xem người dùng đã hoàn thành đơn hàng cho sản phẩm này hay không
                return ordersResponse.Orders.Any(order =>
                    order.CustomerId == userId &&
                    order.OrderItems.Any(item => item.ProductId == Guid.Parse(productId) && order.Status == OrderStatus.Completed)); 
            }

            throw new Exception("Failed to fetch orders from the external service.");
        }



        /*[HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] Review review)
        {
            // Lấy UserId từ header
            var userId = HttpContext.Request.Headers["UserId"].ToString();
            review.RateBy = userId; // Gán UserId vào trường RateBy của Review

            // Kiểm tra xem người dùng đã hoàn thành đơn hàng chưa qua Order service
            var orderCompleted = await HasUserCompletedOrderAsync(userId, review.ProductId);
            if (!orderCompleted)
            {
                return BadRequest("User has not completed the order for this product.");
            }

            review.DatePost = DateTime.UtcNow;
            var createdFeedback = await _feedbackRepository.AddFeedbackAsync(review);

            return CreatedAtAction(nameof(GetFeedbacksByProductId), new { productId = review.ProductId }, createdFeedback);
        }



        [HttpPut("{feedbackId}")]
        public async Task<IActionResult> EditFeedback(string reviewId, [FromBody] Review review)
        {
            var existingFeedback = await _feedbackRepository.GetFeedbackByFeedbackIdAsync(reviewId);
            if (existingFeedback == null)
            {
                return NotFound("Feedback not found.");
            }

            // Kiểm tra quyền chỉnh sửa (nếu cần, ví dụ chỉ người tạo feedback mới được phép sửa)
            if (existingFeedback.UserId != review.UserId)
            {
                return Unauthorized("You are not authorized to edit this feedback.");
            }

            // Cập nhật các trường cần thiết
            existingFeedback.Description = review.Description;
            existingFeedback.Rating = review.Rating;
            existingFeedback.ImageUrl = review.ImageUrl;
            existingFeedback.DatePost = DateTime.UtcNow;

            await _feedbackRepository.UpdateFeedbackAsync(existingFeedback);

            return NoContent();
        }*/

       /* [HttpGet("user/{userId}/feedbacks")]
        public async Task<IActionResult> GetFeedbacksByUserId(string userId)
        {
            // Gọi Product service để lấy danh sách sản phẩm mà user đã mua
            var purchasedProducts = await _productRepository.GetPurchasedProductsByUserIdAsync(userId);

            // Lấy danh sách feedbacks từ repo dựa trên userId
            var feedbacks = await _feedbackRepository.GetFeedbacksByUserIdAsync(userId);

            // Tìm danh sách sản phẩm đã đánh giá
            var reviewedProducts = feedbacks.Select(f => f.ProductId).ToList();

            // Lọc ra những sản phẩm chưa được đánh giá
            var notReviewedProducts = purchasedProducts
                                        .Where(p => !reviewedProducts.Contains(p.ProductId))
                                        .ToList();

            var result = new
            {
                ReviewedProducts = feedbacks,
                NotReviewedProducts = notReviewedProducts
            };

            return Ok(result);
        }*/

    }
}
