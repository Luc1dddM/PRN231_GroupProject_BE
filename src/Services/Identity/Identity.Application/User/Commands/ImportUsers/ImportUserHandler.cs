using BuildingBlocks.CQRS;
using Identity.Application.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands.ImportUsers
{
    public class ImportUserHandler : ICommandHandler<ImportUserCommand, ImportUserResponse>
    {
        private readonly IUserService _userService;
        public ImportUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ImportUserResponse> Handle(ImportUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _userService.ImportUserTemplate(request.file, request.userId);
            return new ImportUserResponse(result);
        }
    }
}
