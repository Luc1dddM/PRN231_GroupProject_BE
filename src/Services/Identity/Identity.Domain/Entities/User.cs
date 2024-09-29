using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Domain.Entities
{
    public class User : IdentityUser
    {
        public required string FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? CreatedAt { get; private set; }
        public string? CreatedBy { get; private set; }
        public string? UpdatedBy { get; private set; }
        public string? UpdatedAt { get; private set; }
    }
}
