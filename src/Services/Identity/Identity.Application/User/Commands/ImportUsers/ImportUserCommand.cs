using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.User.Commands.ImportUsers
{
    public record ImportUserCommand(IFormFile file, string userId) : ICommand<ImportUserResponse>;
    public record ImportUserResponse(BaseResponse<MemoryStream> result);
}
