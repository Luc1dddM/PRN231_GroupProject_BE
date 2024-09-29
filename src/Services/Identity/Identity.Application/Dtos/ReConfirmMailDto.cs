using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs
{
    public class ReConfirmMailDto
    {
        [EmailAddress] public string EmailAddress { get; set; } = string.Empty;
    }
}
