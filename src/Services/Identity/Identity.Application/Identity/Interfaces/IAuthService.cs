using BuildingBlocks.Models;
using Identity.Application.Identity.Dtos;

namespace Identity.Application.Identity.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<LoginReponseDto>> SignInWithGoogle(string idToken);
        Task<BaseResponse<CreateCustomerDto>> Register(string email, string name, string phonenumber, string password);
        Task<BaseResponse<LoginReponseDto>> Login(string username, string password);
        Task<BaseResponse<string>> ConfirmEmail(string userId, string token);
        Task<BaseResponse<string>> ReConfirmEmail(string emailaddress);
        Task<BaseResponse<JwtModelVM>> RenewToken(JwtModelVM request);
        Task<BaseResponse<string>> ForgotPassword(string email);
        Task<BaseResponse<string>> ForgotPasswordConfirm(string email, string code);


    }
}
