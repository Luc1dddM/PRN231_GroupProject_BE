using BuildingBlocks.CQRS;
using Identity.Application.User.Interfaces;

namespace Identity.Application.User.Commands.GetUserById
{
    public class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, GetUserByIdResponse>
    {
        private readonly IUserService _userService;

        public GetUserByIdHandler(IUserService service) { 
            _userService = service;
        } 
        public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var reponse = await _userService.GetUserById(request.id);
            return new GetUserByIdResponse(reponse);
        }
    }
}
