using System.ComponentModel.DataAnnotations;

namespace Chat.API.Model.DTO
{
    public class CreateUserConsumer
    {
        public string UserId { get; set; } = default!;
        public bool IsCustomer { get; set; }
        public string Name { get; set; } = default!;
    }
}
