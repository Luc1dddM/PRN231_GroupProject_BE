using Identity.Application.Identity.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<JwtResponseVM>> SignInWithGoogle(string idToken);
        Task<BaseResponse<CreateCustomerDto>> Register(string email, string name, string phonenumber, string password);
        Task<BaseResponse<JwtResponseVM>> Login(string username, string password);
        Task<BaseResponse<string>> ConfirmEmail(string userId, string token);
        Task<BaseResponse<string>> ReConfirmEmail(string emailaddress);
    }
}
