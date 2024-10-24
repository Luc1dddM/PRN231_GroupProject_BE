
using BuildingBlocks.Messaging.Events;
using FluentValidation;
using MassTransit;

namespace ShoppingCart.API.ShoppingCart.CheckoutCart
{
    public record CheckoutCartCommand(CartCheckoutDto CartCheckoutDto) : ICommand<CheckoutCartResult>;
    public record CheckoutCartResult(BaseResponse<object> Result);


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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckoutCartHandler(ICartRepository cartRepository, IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CheckoutCartResult> Handle(CheckoutCartCommand command, CancellationToken cancellationToken)
        {
            // get existing basket with total price
            // Set totalprice on basketcheckout event message
            // send basket checkout event to rabbitmq using masstransit
            // delete the basket

            var userId = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            if (userId == null)
            {
                throw new NotFoundException("UserId did not have any value in the incoming request.");
            }


            var cart = await _cartRepository.GetCart(userId);
            cart.CartHeader.TotalPrice = cart.CartDetails.Sum(c => c.Price * c.Quantity);

            // Create a new DTO that combines the request data with database cart details
            var completeCartCheckoutDto = MapCartFromDbToCartCheckoutDto(command.CartCheckoutDto, cart);


            var eventMessage = MapToCartCheckoutEvent(completeCartCheckoutDto);
            eventMessage.TotalPrice = (decimal)cart.CartHeader.TotalPrice;

            await _publishEndpoint.Publish(eventMessage, cancellationToken);
            await _cartRepository.DeleteCart(userId, cancellationToken);

            return new CheckoutCartResult(new BaseResponse<object>
            {
                IsSuccess = true,
                Result = eventMessage,
                Message = "Cart Checkout Successful."
            });


        }

        private CartCheckoutDto MapCartFromDbToCartCheckoutDto(CartCheckoutDto requestDto, CartDto dbCart)
        {
            var userId = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            if (userId == null)
            {
                throw new NotFoundException("UserId did not have any value in the incoming request.");
            }

            var completeCartDto = new CartCheckoutDto
            {
                CustomerId = Guid.Parse(userId),
                FirstName = requestDto.FirstName,
                LastName = requestDto.LastName,
                Phone = requestDto.Phone,
                EmailAddress = requestDto.EmailAddress,
                AddressLine = requestDto.AddressLine,
                City = requestDto.City,
                District = requestDto.District,
                Ward = requestDto.Ward,
                Payment = requestDto.Payment,
                CouponCode = requestDto.CouponCode
            };

            completeCartDto.CartDetails = dbCart.CartDetails.Select(detail => new CartDetailCheckoutDto
            {
                ProductId = detail.ProductId,
                ProductName = detail.ProductName,
                Quantity = detail.Quantity,
                Color = detail.Color,
                Price = (decimal)detail.Price,
                ProductCategoryId = detail.ProductCategoryId
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
                    Price = detail.Price,
                    ProductCategoryId = detail.ProductCategoryId
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
                Payment = dto.Payment,
            };
        }
    }
}
