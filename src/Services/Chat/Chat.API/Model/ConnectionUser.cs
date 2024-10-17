using System.ComponentModel.DataAnnotations;

namespace Chat.API.Model
{
    public class ConnectionUser
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required] 
        public bool IsCustomer { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<UserMessage> UserMessages { get; set; } = new List<UserMessage>();


    }
}
    