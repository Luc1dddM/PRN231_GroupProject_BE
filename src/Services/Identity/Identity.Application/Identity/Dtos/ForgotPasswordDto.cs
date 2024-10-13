using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Dtos
{
    public class ForgotPasswordDto
    {
        [EmailAddress] public string EmailAddress { get; set; } = string.Empty;
    }
}
