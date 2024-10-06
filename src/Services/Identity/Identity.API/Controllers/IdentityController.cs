using Identity.Application.Identity;
using Identity.Application.Identity.Commands.ConfirmEmail;
using Identity.Application.Identity.Commands.GoogleLogin;
using Identity.Application.Identity.Commands.InternalLogin;
using Identity.Application.Identity.Commands.ReconfirmEmail;
using Identity.Application.Identity.Commands.Register;
using Identity.Application.Identity.Commands.RenewToken;
using Identity.Application.Identity.Dtos;
using Identity.Application.Identity.Interfaces;
using Identity.Application.RolePermission.Commands.AddRole;
using Identity.Application.RolePermission.Commands.UpdatePermission;
using Identity.Application.RolePermission.Commands.UpdateRoles;
using Identity.Application.RolePermission.Dtos;
using Identity.Application.Utils;
using Identity.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IdentityController : BaseController
    {
        private readonly IMediator _mediator;
        public IdentityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> GoogleSignIn(GoogleSignInVM request)
        {
            try
            {
                var command = request.Adapt<GoogleLoginQuery>();
                var reponse = await _mediator.Send(command);
                return ReturnResponse(reponse.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
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
                return ReturnResponse(result.result);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
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
                return ReturnResponse(result.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
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
                return ReturnResponse(result.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
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
                return ReturnResponse(result.result);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
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
                return ReturnResponse(result.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
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
                return ReturnResponse(result.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
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
                return ReturnResponse(result.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDto request)
        {
            try
            {
                var userId = HttpContext.Request.Headers["UserId"].ToString();
                if (userId == null) HandleError(new Exception(), "User Id Is Null" );
                var role = new Role()
                {
                    Name = request.name,
                    CreatedBy = userId,
                };
                var command = role.Adapt<AddRoleCommand>();
                var result = await _mediator.Send(command);
                return ReturnResponse(result.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}
