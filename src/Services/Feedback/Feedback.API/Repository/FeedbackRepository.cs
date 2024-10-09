using Feedback.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Feedback.API.Repository
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly Prn231GroupProjectContext _context;

        public FeedbackRepository(Prn231GroupProjectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetFeedbacksByProductIdAsync(string productId)
        {
            return await _context.Reviews
                .Where(f => f.ProductId == productId)
                .ToListAsync();
        }

        /*public async Task<Review?> GetFeedbackByFeedbackIdAsync(string reviewId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(f => f.FeedbackId == reviewId);
        }*/

        public async Task<Review> AddFeedbackAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task UpdateFeedbackAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetFeedbacksByUserIdAsync(string userId)
        {
            return await _context.Reviews
                .Where(f => f.RateBy == userId)
                .ToListAsync();
        }

        // Thêm phương thức này để lấy danh sách tất cả feedbacks
        public async Task<IEnumerable<Review>> GetFeedbackListAsync()
        {
            return await _context.Reviews.ToListAsync();
        }
    }
}
