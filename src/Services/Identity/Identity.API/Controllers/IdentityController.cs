using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Identity.Application.Identity;
using Identity.Application.Identity.Commands.ConfirmEmail;
using Identity.Application.Identity.Commands.GoogleLogin;
using Identity.Application.Identity.Commands.InternalLogin;
using Identity.Application.Identity.Commands.ReconfirmEmail;
using Identity.Application.Identity.Commands.Register;
using Identity.Application.Identity.Commands.RenewToken;
using Identity.Application.Identity.Commands.ResetPassword;
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

            var command = request.Adapt<GoogleLoginQuery>();
            var reponse = await _mediator.Send(command);
            return Ok(reponse.response);

        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = request.Adapt<RegisterCommand>();
            var result = await _mediator.Send(command);
            return Ok(result.result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = request.Adapt<InternalLoginQuery>();
            var result = await _mediator.Send(query);
            return Ok(result.response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> RenewToken([FromBody] JwtModelVM request)
        {
            var command = new RenewTokenCommand(request);
            var result = await _mediator.Send(command);
            return Ok(result.response);
        }


        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> ConfirmEmail(string UserId, string Token)
        {
            var command = new ConfirmEmailCommand(UserId, Token);
            var result = await _mediator.Send(command);
            return Ok(result.result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var query = new ResetPasswordQuery(dto.EmailAddress);
            var result = await _mediator.Send(query);
            return Ok(result.response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> ReConfirmAccount([FromBody] ReConfirmMailDto request)
        {

            var query = request.Adapt<ReconfirmCommand>();
            var result = await _mediator.Send(query);
            return Ok(result.response);

        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> UpdatePermission([FromBody] UpdatePermissionDto request)
        {
            var command = request.Adapt<UpdatePermissionCommand>();
            var result = await _mediator.Send(command);
            return Ok(result.response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> UpdateRoles([FromBody] UpdateRolesDto request)
        {
            var command = request.Adapt<UpdateRolesCommand>();
            var result = await _mediator.Send(command);
            return Ok(result.response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDto request)
        {
            var command = new AddRoleCommand(request.name);
            var result = await _mediator.Send(command);
            return Ok(result.response);
        }
    }
}
