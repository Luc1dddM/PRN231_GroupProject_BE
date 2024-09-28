using Identity.Application.DTOs;
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
        Task<BaseResponse<JwtResponseVM>> SignInWithGoogle(GoogleSignInVM model);
        Task<BaseResponse<JwtResponseVM>> SignInWithFacebook(FacebookSignInVM model);
        Task<BaseResponse<UserDto>> Register(RegistrationRequestDto registrationRequest);
        Task<BaseResponse<JwtResponseVM>> Login(LoginRequestDto loginRequest);
        Task<BaseResponse<string>> ConfirmEmail(string userId, string token);
        Task<BaseResponse<string>> ReConfirmEmail(ReConfirmMailDto reConfirmMailDto);
    }
}
