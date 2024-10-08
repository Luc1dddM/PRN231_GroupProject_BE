using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Identity.Application.Identity;
using Identity.Application.Identity.Commands.ConfirmEmail;
using Identity.Application.Identity.Commands.GoogleLogin;
using Identity.Application.Identity.Commands.InternalLogin;
using Identity.Application.Identity.Commands.ReconfirmEmail;
using Identity.Application.Identity.Commands.Register;
using Identity.Application.Identity.Commands.RenewToken;
using Identity.Application.Identity.Dtos;
using Identity.Application.RolePermission.Commands.AddRole;
using Identity.Application.RolePermission.Commands.UpdatePermission;
using Identity.Application.RolePermission.Commands.UpdateRoles;
using Identity.Application.RolePermission.Dtos;
using Identity.Application.User.Dtos;
using Identity.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator _mediator;
        public IdentityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> GoogleSignIn(GoogleSignInVM request)
        {
            try
            {
                var command = request.Adapt<GoogleLoginQuery>();
                var reponse = await _mediator.Send(command);
                return Ok(reponse.response);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var command = request.Adapt<RegisterCommand>();
                var result = await _mediator.Send(command);
                return Ok(result.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = request.Adapt<InternalLoginQuery>();
                var result = await _mediator.Send(query);
                return Ok(result.response);
            }
            catch (UnAuthorizeException ex)
            {
                return StatusCode(401, new BaseResponse<UserDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new BaseResponse<UserDto>
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> RenewToken([FromBody] JwtModelVM request)
        {
            try
            {
                var command = new RenewTokenCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result.response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> ConfirmEmail(string UserId, string Token)
        {
            try
            {
                var command = new ConfirmEmailCommand(UserId, Token);
                var result = await _mediator.Send(command);
                return Ok(result.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> ReConfirmAccount([FromBody] ReConfirmMailDto request)
        {
            try
            {
                var query =  request.Adapt<ReconfirmCommand>();
                var result = await _mediator.Send(query);
                return Ok(result.response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> UpdatePermission([FromBody] UpdatePermissionDto request)
        {
            try
            {
                var command = request.Adapt<UpdatePermissionCommand>();
                var result = await _mediator.Send(command);
                return Ok(result.response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> UpdateRoles([FromBody] UpdateRolesDto request)
        {
            try
            {
                var command = request.Adapt<UpdateRolesCommand>();
                var result = await _mediator.Send(command);
                return Ok(result.response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDto request)
        {
            try
            {
                var userId = HttpContext.Request.Headers["UserId"].ToString();
                if (userId == null) BadRequest( "User Id Is Null" );
                var role = new Role()
                {
                    Name = request.name,
                    CreatedBy = userId,
                };
                var command = role.Adapt<AddRoleCommand>();
                var result = await _mediator.Send(command);
                return Ok(result.response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
