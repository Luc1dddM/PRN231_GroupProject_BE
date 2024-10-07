
using FluentValidation;
using MassTransit;

namespace ShoppingCart.API.ShoppingCart.CheckoutCart
{
    public record CheckoutcartCommand(CartCheckoutDto CartCheckoutDto) : ICommand<CheckoutcartResult>;
    public record CheckoutcartResult(bool IsSuccess);


    public class CheckoutCartCommandValidator : AbstractValidator<CheckoutcartCommand>
    {
        public CheckoutCartCommandValidator()
        {
            RuleFor(x => x.CartCheckoutDto).NotNull().WithMessage("BasketCheckoutDto can't be null");
            RuleFor(x => x.CartCheckoutDto.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }


    public class CheckoutCartHandler : ICommandHandler<CheckoutcartCommand, CheckoutcartResult>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public CheckoutCartHandler(ICartRepository cartRepository, IPublishEndpoint publishEndpoint)
        {
            _cartRepository = cartRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<CheckoutcartResult> Handle(CheckoutcartCommand request, CancellationToken cancellationToken)
        {
            // get existing basket with total price
            // Set totalprice on basketcheckout event message
            // send basket checkout event to rabbitmq using masstransit
            // delete the basket
            throw new NotImplementedException();
        }
    }
}
