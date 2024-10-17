using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.API.Model
{
    public class Message
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string MessageId { get; set; } = default!;
        [Required]
        public string Content { get; set; } = default!;
        [Required]
        public string SenderId { get; set; } = default!;
        [ForeignKey("UserId")]
        [Required]
        public string GroupId { get; set; } = default!;
        [ForeignKey("GroupId")]
        [Required]
        public DateTime CreateAt { get; set; }

        public virtual ConnectionUser User { get; set; } = default!;
        public virtual Group Group { get; set; } = default!;
        public virtual ICollection<UserMessage> UserMessages { get; set; } = new List<UserMessage>();

    }
}
