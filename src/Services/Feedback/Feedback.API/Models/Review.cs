using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback.API.Models
{
    public class Review
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FeedbackId { get; set; } = Guid.NewGuid().ToString();

        public string Description { get; set; }

        public string ProductId { get; set; }

        public string OrderId { get; set; }

        public int Rating { get; set; }

        public string ImageUrl { get; set; }

        public string RateBy { get; set; } = null!;

        public DateTime DatePost { get; set; }
    }
}
