using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands.ExportUsers
{
    public class ExportUsersHandler : IQueryHandler<ExportUsersQuery, ExportUsersResponse>
    {
        private readonly IUserService _userService;
        public ExportUsersHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<ExportUsersResponse> Handle(ExportUsersQuery request, CancellationToken cancellationToken)
        {
            var response = await _userService.ExportUser(request.parameters);
            return new ExportUsersResponse(response);
        }
    }
}
