using BuildingBlocks.Models;
using Identity.Application.User.Dtos;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.User.Interfaces
{
    public interface IUserService
    {
        public Task<BaseResponse<PaginatedList<UserDto>>> GetAllUser(GetListUserParamsDto parameters);
        public Task<BaseResponse<UserDto>> GetUserById(string id); 
        public Task<BaseResponse<UserDto>> CreateNewUser(CreateNewUserDto dto);
        public Task<BaseResponse<UserDto>> UpdateUser(string id, UpdateUserDto request);
        public Task<BaseResponse<MemoryStream>> ImportUserTemplate(IFormFile file, string userId);
    }
}
