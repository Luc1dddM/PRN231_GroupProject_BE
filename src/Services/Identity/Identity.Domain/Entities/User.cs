using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Domain.Entities
{
    public class User : IdentityUser
    {
        public required string FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsActive{ get; set; } = true;
        public DateOnly BirthDay { get; set; } = DateOnly.FromDateTime(DateTime.Now);  
        public DateTime? CreatedAt { get;  set; }
        public string? CreatedBy { get;  set; }
        public string? UpdatedBy { get;  set; }
        public DateTime? UpdatedAt { get;  set; }
    }
}
