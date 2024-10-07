using Identity.Application.Identity.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Interfaces
{
    public interface IFacebookAuthService
    {
        Task<BaseResponse<FacebookTokenValidationResponse>> ValidateFacebookToken(string accessToken);
        Task<BaseResponse<FacebookUserInfoResponse>> GetFacebookUserInformation(string accessToken);
    }
}
