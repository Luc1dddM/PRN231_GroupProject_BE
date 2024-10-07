using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Identity.Dtos
{
    public class FacebookSignInVM
    {
        /// <summary>
        /// This token is generated from the client side. i.e. react, angular, flutter etc.
        /// </summary>
        [Required]
        public string AccessToken { get; set; }
    }
}
