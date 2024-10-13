using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.Identity.Dtos;

namespace Identity.Application.Identity.Commands.Register;
public record RegisterCommand(string Email, string Name,string Phonenumber, string Password) : ICommand<RegisterResult>;

public record RegisterResult(BaseResponse<CreateCustomerDto> result);