
using BuildingBlocks.Messaging.Events;
using FluentValidation;
using MassTransit;

namespace ShoppingCart.API.ShoppingCart.CheckoutCart
{
    public record CheckoutCartCommand(CartCheckoutDto CartCheckoutDto) : ICommand<CheckoutCartResult>;
    public record CheckoutCartResult(bool IsSuccess);


    public class CheckoutCartCommandValidator : AbstractValidator<CheckoutCartCommand>
    {
        public CheckoutCartCommandValidator()
        {
            RuleFor(x => x.CartCheckoutDto).NotNull().WithMessage("BasketCheckoutDto can't be null");
            RuleFor(x => x.CartCheckoutDto.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }


    public class CheckoutCartHandler : ICommandHandler<CheckoutCartCommand, CheckoutCartResult>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public CheckoutCartHandler(ICartRepository cartRepository, IPublishEndpoint publishEndpoint)
        {
            _cartRepository = cartRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<CheckoutCartResult> Handle(CheckoutCartCommand command, CancellationToken cancellationToken)
        {
            // get existing basket with total price
            // Set totalprice on basketcheckout event message
            // send basket checkout event to rabbitmq using masstransit
            // delete the basket

            var cart = await _cartRepository.GetCart(command.CartCheckoutDto.CustomerId.ToString());
            if (cart == null)
            {
                return new CheckoutCartResult(false);
            }

            // Create a new DTO that combines the request data with database cart details
            var completeCartCheckoutDto = MapCartFromDbToCartCheckoutDto(command.CartCheckoutDto, cart);


            var eventMessage = MapToCartCheckoutEvent(completeCartCheckoutDto);
            eventMessage.TotalPrice = (decimal)cart.CartHeader.TotalPrice;

            await _publishEndpoint.Publish(eventMessage, cancellationToken);
            await _cartRepository.DeleteCart(command.CartCheckoutDto.CustomerId.ToString(), cancellationToken);

            return new CheckoutCartResult(true);
        }

        private static CartCheckoutDto MapCartFromDbToCartCheckoutDto(CartCheckoutDto requestDto, CartDto dbCart)
        {
            var completeCartDto = new CartCheckoutDto
            {
                CustomerId = requestDto.CustomerId,
                FirstName = requestDto.FirstName,
                LastName = requestDto.LastName,
                Phone = requestDto.Phone,
                EmailAddress = requestDto.EmailAddress,
                AddressLine = requestDto.AddressLine,
                City = requestDto.City,
                District = requestDto.District,
                Ward = requestDto.Ward,
                CardName = requestDto.CardName,
                CardNumber = requestDto.CardNumber,
                Expiration = requestDto.Expiration,
                CVV = requestDto.CVV,
                PaymentMethod = requestDto.PaymentMethod,
                CouponCode = requestDto.CouponCode
            };

            completeCartDto.CartDetails = dbCart.CartDetails.Select(detail => new CartDetailCheckoutDto
            {
                ProductId = detail.ProductId,
                ProductName = detail.ProductName,
                Quantity = detail.Quantity,
                Color = detail.Color,
                Price = (decimal)detail.Price
            }).ToList();

            completeCartDto.TotalPrice = (decimal)dbCart.CartHeader.TotalPrice;

            return completeCartDto;
        }

        private static CartCheckoutEvent MapToCartCheckoutEvent(CartCheckoutDto dto)
        {
            return new CartCheckoutEvent
            {
                CustomerId = dto.CustomerId,

                CartItems = dto.CartDetails.Select(detail => new CartDetailEvent
                {
                    ProductId = detail.ProductId,
                    ProductName = detail.ProductName,
                    Quantity = detail.Quantity,
                    Color = detail.Color,
                    Price = detail.Price
                }).ToList(),

                CouponCode = dto.CouponCode,

                //shipping address
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                EmailAddress = dto.EmailAddress,
                AddressLine = dto.AddressLine,
                City = dto.City,
                District = dto.District,
                Ward = dto.Ward,

                //payment
                CardName = dto.CardName,
                CardNumber = dto.CardNumber,
                Expiration = dto.Expiration,
                CVV = dto.CVV,
                PaymentMethod = dto.PaymentMethod
            };
        }
    }
}
