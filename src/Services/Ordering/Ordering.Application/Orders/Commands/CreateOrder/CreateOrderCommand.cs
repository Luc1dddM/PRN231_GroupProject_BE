using BuildingBlocks.Models;
using FluentValidation;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(OrderDtoRequest Order) : ICommand<CreateOrderResult>;

    public record CreateOrderResult(BaseResponse<OrderDto> Result);

    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.Order.CustomerId).NotNull().WithMessage("CustomerId is required");
            RuleFor(x => x.Order.OrderItems).NotEmpty().WithMessage("OrderItems should not be empty");
        }
    }
}
