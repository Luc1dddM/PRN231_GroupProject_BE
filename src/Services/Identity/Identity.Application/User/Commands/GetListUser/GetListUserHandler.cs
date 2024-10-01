using BuildingBlocks.CQRS;
using Identity.Application.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands.GetListUser
{
    public class GetListUserHandler : IQueryHandler<GetListUsersQuery, GetListUsersResponse>
    {
        private readonly IUserService _serivce;

        public GetListUserHandler(IUserService serivce)
        {
            _serivce = serivce;
        }
        public async Task<GetListUsersResponse> Handle(GetListUsersQuery request, CancellationToken cancellationToken)
        {
            var reponse = await _serivce.GetAllUser(request.parameters);
            return new GetListUsersResponse(reponse);
        }
    }
}
