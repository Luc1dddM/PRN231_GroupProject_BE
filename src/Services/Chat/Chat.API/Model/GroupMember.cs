using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.API.Model
{
    public class GroupMember
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string GroupMemberId { get; set; }
        [Required]
        public string GroupId { get; set; }
        [ForeignKey("GroupId")]
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [Required]
        public DateTime AddedDate { get; set; }
        public string? AddBy { get; set; }

        public virtual Group Group { get; set; }
        public virtual ConnectionUser User { get; set; }


    }
}
