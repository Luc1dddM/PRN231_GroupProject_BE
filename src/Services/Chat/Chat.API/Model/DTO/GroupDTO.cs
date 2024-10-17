using System.ComponentModel.DataAnnotations;

namespace Chat.API.Model.DTO
{
    public class GroupDTO
    {
        public string GroupId { get; set; } = default!;
        public string GroupName { get; set; } = default!;
    }
}
