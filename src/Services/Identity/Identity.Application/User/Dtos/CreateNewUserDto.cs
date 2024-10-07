using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Dtos
{
    public class CreateNewUserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }
        public IFormFile? ImageFile { get; set; }
        public required string FullName { get; set; }
        public required bool IsActive { get; set; }
        public required DateOnly BirthDay {  get; set; }
        public required List<string> Role { get; set; }
        public string? CreatedBy { get; set; }
    }
}
