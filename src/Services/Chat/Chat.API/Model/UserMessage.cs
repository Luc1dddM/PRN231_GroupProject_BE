using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.API.Model
{
    public class UserMessage
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserMessageId { get; set; } = default!;
        [Required]
        public string MessageId { get; set; } = default!;
        [ForeignKey("MessageId")]
        [Required]
        public string ReceiverId { get; set; } = default!;
        [ForeignKey("UserId")]
        [Required]
        public bool Status { get; set; }

        public virtual Message Message { get; set; } = default!;
        public virtual ConnectionUser ConnectionUser { get; set; } = default!;
    }
}
