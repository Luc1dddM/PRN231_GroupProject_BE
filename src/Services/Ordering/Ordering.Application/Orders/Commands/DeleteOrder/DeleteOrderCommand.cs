using BuildingBlocks.Models;
using FluentValidation;

namespace Ordering.Application.Orders.Commands.DeleteOrder
{

    public record DeleteOrderCommand(Guid EntityId) : ICommand<DeleteOrderResult>;

    public record DeleteOrderResult(BaseResponse<object> Result);

    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(x => x.EntityId).NotEmpty().WithMessage("OrderId is required");
        }
    }
}
