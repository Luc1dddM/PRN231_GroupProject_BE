using Feedback.API.Models;

namespace Feedback.API.Repository;

public interface IFeedbackRepository
{
    Task<IEnumerable<Review>> GetFeedbacksByProductIdAsync(string productId);
/*    Task<Review?> GetFeedbackByFeedbackIdAsync(string reviewId);*/
    Task<Review> AddFeedbackAsync(Review review);
    Task UpdateFeedbackAsync(Review review);

    Task<IEnumerable<Review>> GetFeedbackListAsync();

    Task<IEnumerable<Review>> GetFeedbacksByUserIdAsync(string userId);
}
