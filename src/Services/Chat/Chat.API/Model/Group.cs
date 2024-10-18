using System.ComponentModel.DataAnnotations;

namespace Chat.API.Model
{
    public class Group
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string GroupName { get; set; }

        public virtual ICollection<GroupMember> GroupMembers{ get; set; } = new List<GroupMember>();
        public virtual ICollection<Message> Messages{ get; set; } = new List<Message>();


    }
}
