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
        public string MessageId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string SenderId { get; set; }
        [ForeignKey("UserId")]
        [Required]
        public string GroupId { get; set; }
        [ForeignKey("GroupId")]
        [Required]
        public DateTime CreateAt { get; set; }

        public virtual ConnectionUser User { get; set; }
        public virtual Group Group { get; set; }


    }
}
