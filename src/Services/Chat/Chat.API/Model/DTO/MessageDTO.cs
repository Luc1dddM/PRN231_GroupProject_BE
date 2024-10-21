using System.ComponentModel.DataAnnotations;

namespace Chat.API.Model.DTO
{
    public class MessageDTO
    {
        public string Content { get; set; } = default!;
        public string SenderId { get; set; } = default!;
        public string SenderName { get; set; } = default!;
    }
}
