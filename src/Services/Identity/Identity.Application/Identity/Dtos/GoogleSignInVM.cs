using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Identity.Dtos
{
    public record GoogleSignInVM
    {
        /// <summary>
        /// This token being passed here is generated from the client side when a request is made  to 
        /// i.e. react, angular, flutter etc. It is being returned as A jwt from google oauth server. 
        /// </summary>
        [Required]
        public string Token { get; set; }
    }
}
