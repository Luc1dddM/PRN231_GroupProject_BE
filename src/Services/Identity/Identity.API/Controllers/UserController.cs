using Identity.Application.Identity.Commands.GoogleLogin;
using Identity.Application.User.Commands.CreateUser;
using Identity.Application.User.Commands.GetListUser;
using Identity.Application.User.Commands.Updateuser;
using Identity.Application.User.Dtos;
using Identity.Application.Utils;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> GetListUsers([FromQuery]  GetListUserParamsDto request, CancellationToken cancellation = default)
        {
            try
            {
                var query = new GetListUsersQuery(request);
                var reponse = await _mediator.Send(query);
                return ReturnResponse(reponse.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> CreateNewUser([FromForm] CreateNewUserDto Request, CancellationToken cancellation = default)
        {
            try
            {
                var userId = HttpContext.Request.Headers["UserId"].ToString();
                if (string.IsNullOrEmpty(userId)) return HandleError(new Exception("User Id Is Null"));
                Request.CreatedBy = userId;
                var query = new CreateUserCommand(Request);
                var reponse = await _mediator.Send(query);
                return ReturnResponse(reponse.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UpdateUserDto Request, CancellationToken cancellation = default)
        {
            try
            {
                var userId = HttpContext.Request.Headers["UserId"].ToString();
                if (string.IsNullOrEmpty(userId)) return HandleError(new Exception("User Id Is Null"));
                Request.UpdatedBy = userId;
                var query = new UpdateUserCommand(id, Request);
                var reponse = await _mediator.Send(query);
                return ReturnResponse(reponse.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}
