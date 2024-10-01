using Identity.Application.Identity.Dtos;
using Identity.Application.Utils;
using Identity.Domain.Entities;
namespace Identity.Application.Identity.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<BaseResponse<Domain.Entities.User>> GoogleSignIn(string idToken);
    }
}
