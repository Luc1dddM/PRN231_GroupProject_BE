using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Dtos
{
    public class UpdateUserDto
    {
        public required string Id { get; set; }
        public  string? Email { get; set; }
        public  string? PhoneNumber { get; set; }
        public IFormFile? ImageFile { get; set; }
        public  string? FullName { get; set; }
        public  string? Gender { get; set; }
        public  bool? IsActive { get; set; }
        public  DateOnly BirthDay { get; set; }
        public List<string>? Roles { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
