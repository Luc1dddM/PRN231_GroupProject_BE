using BuildingBlocks.Models;
using FluentValidation;

namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    public record UpdateOrderCommand(OrderDtoUpdateRequest Order) : ICommand<UpdateOrderResult>;

    public record UpdateOrderResult(BaseResponse<OrderDto> Result);


    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator() 
        {
            RuleFor(x => x.Order.EntityId).NotEmpty().WithMessage("Id is required");
            RuleFor(x => x.Order.CustomerId).NotNull().WithMessage("CustomerId is required");
        }
    }
}
