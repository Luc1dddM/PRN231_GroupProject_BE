using BuildingBlocks.Models;
using Identity.Application.User.Dtos;
using Identity.Application.Utils;

namespace Identity.Application.User.Interfaces
{
    public interface IUserService
    {
        public Task<BaseResponse<PaginatedList<Domain.Entities.User>>> GetAllUser(GetListUserParamsDto parameters);
        public Task<BaseResponse<Domain.Entities.User>> CreateNewUser(CreateNewUserDto dto);
        public Task<BaseResponse<bool>> UpdateUser(string id, UpdateUserDto request);
    }
}
