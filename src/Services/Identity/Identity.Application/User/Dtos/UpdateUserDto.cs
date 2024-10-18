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
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public IFormFile? ImageFile { get; set; }
        public required string FullName { get; set; }
        public required bool IsActive { get; set; }
        public required DateOnly BirthDay { get; set; }
        public required List<string> Roles { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
