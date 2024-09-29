using BuildingBlocks.CQRS;
using Identity.Application.DTOs;
using Identity.Application.Utils;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.Register;
public record RegisterCommand(RegistrationRequestDto request) : ICommand<RegisterResult>;

public record RegisterResult(BaseResponse<UserDto> result);