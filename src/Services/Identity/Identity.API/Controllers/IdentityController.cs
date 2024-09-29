using Identity.Application.Dtos;
using Identity.Application.DTOs;
using Identity.Application.Identity.Commands.ConfirmEmail;
using Identity.Application.Identity.Commands.GoogleLogin;
using Identity.Application.Identity.Commands.InternalLogin;
using Identity.Application.Identity.Commands.ReconfirmEmail;
using Identity.Application.Identity.Commands.Register;
using Identity.Application.Identity.Interfaces;
using Identity.Application.RolePermission.Commands.AddRole;
using Identity.Application.RolePermission.Commands.UpdatePermission;
using Identity.Application.Utils;
using Identity.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IdentityController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        public IdentityController(IMediator mediator, IAuthService authService)
        {
            _mediator = mediator;
            _authService = authService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> GoogleSignIn(GoogleLoginQuery request, CancellationToken cancellation = default)
        {
            try
            {
                var reponse = await _mediator.Send(request);
                return ReturnResponse(reponse.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _mediator.Send(request);
                return ReturnResponse(result.result);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> Login([FromBody] InternalLoginQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _mediator.Send(request);
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
                var request = new ConfirmEmailCommand(UserId, Token);
                var result = await _mediator.Send(request);
                return ReturnResponse(result.result);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> ReConfirmAccount([FromBody] ReconfirmCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return ReturnResponse(result.response);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> UpdatePermission([FromBody] UpdatePermissionCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);
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
