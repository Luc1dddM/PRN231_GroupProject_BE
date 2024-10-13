using BuildingBlocks.Models;
using Identity.Application.User.Commands.CreateUser;
using Identity.Application.User.Commands.GetListUser;
using Identity.Application.User.Commands.GetUserById;
using Identity.Application.User.Commands.Updateuser;
using Identity.Application.User.Dtos;
using Identity.Infrastructure.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> GetListUsers([FromQuery] GetListUserParamsDto request, CancellationToken cancellation = default)
        {
            var query = new GetListUsersQuery(request);
            var reponse = await _mediator.Send(query);
            return Ok(reponse.response);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetUserById(string id, CancellationToken cancellation = default)
        {
            var query = new GetUserByIdQuery(id);
            var reponse = await _mediator.Send(query);
            return Ok(reponse.response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> CreateNewUser([FromForm] CreateNewUserDto Request, CancellationToken cancellation = default)
        {
            var userId = HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId)) return BadRequest(new Exception("User Id Is Null"));
            Request.CreatedBy = userId;
            var query = new CreateUserCommand(Request);
            var reponse = await _mediator.Send(query);
            return Ok(reponse.response);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UpdateUserDto Request, CancellationToken cancellation = default)
        {
            var userId = HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId)) return BadRequest(new Exception("User Id Is Null"));
            Request.UpdatedBy = userId;
            var query = new UpdateUserCommand(id, Request);
            var reponse = await _mediator.Send(query);
            return Ok(reponse.response);
        }
    }
}
