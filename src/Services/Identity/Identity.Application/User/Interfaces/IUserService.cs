using BuildingBlocks.Models;
using Identity.Application.User.Dtos;
using Identity.Application.Utils;

namespace Identity.Application.User.Interfaces
{
    public interface IUserService
    {
        public Task<BaseResponse<PaginatedList<UserDto>>> GetAllUser(GetListUserParamsDto parameters);
        public Task<BaseResponse<UserDto>> GetUserById(string id); 
        public Task<BaseResponse<UserDto>> CreateNewUser(CreateNewUserDto dto);
        public Task<BaseResponse<bool>> UpdateUser(string id, UpdateUserDto request);
    }
}
