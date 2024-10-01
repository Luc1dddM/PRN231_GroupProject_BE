using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Identity
{
    public class ReConfirmMailDto
    {
        [EmailAddress] public string EmailAddress { get; set; } = string.Empty;
    }
}
