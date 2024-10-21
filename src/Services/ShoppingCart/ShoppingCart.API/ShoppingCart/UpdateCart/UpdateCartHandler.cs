
using ShoppingCart.API.ShoppingCart.StoreCart;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ShoppingCart.API.ShoppingCart.UpdateCart
{
    public record UpdateCartCommand(CartHeader CartHeader) : ICommand<UpdateCartResult>;
    public record UpdateCartResult(BaseResponse<CartDto> Result);

    public class UpdateCartHandler : ICommandHandler<UpdateCartCommand, UpdateCartResult>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICartRepository _repository;

        public UpdateCartHandler(IHttpContextAccessor contextAccessor, ICartRepository cartRepository)
        {
            _contextAccessor = contextAccessor;
            _repository = cartRepository;
        }

        public async Task<UpdateCartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
        {
            var userId = _contextAccessor.HttpContext.Request.Headers["UserId"].ToString();

            if (string.IsNullOrEmpty(userId))
            {
                throw new NotFoundException("UserId did not have any value in the incoming request.");
            }

            //checkif the cart header already existing for a user
            var existingHeader = await _repository.GetCartHeaderByUserId(userId, cancellationToken);

            if (existingHeader != null)
            {
                //update cart header
                existingHeader.UpdatedDate = DateTime.Now;
                existingHeader.UpdatedBy = userId;
                await _repository.UpdateCartHeader(existingHeader, userId, cancellationToken);

                foreach (var detail in command.CartHeader.CartDetails)
                {
                    var existingDetail = await _repository.GetCartDetailById(detail.CartDetailId, cancellationToken);

                    if (existingDetail != null)
                    {
                        var sameCartDetail = await _repository.GetCartDetailByCartHeaderId_ProductCategoryId(existingHeader.CartHeaderId, detail.ProductCategoryId);
                        if (sameCartDetail != null &&
                            !existingDetail.ProductCategoryId.Equals(sameCartDetail.ProductCategoryId) &&
                            !existingDetail.CartDetailId.Equals(sameCartDetail.CartDetailId))
                        {
                            sameCartDetail.Quantity += existingDetail.Quantity;
                            await _repository.DeleteCartDetails(existingDetail.CartDetailId);
                            await _repository.UpdateCartDetails(sameCartDetail);
                        }
                        else
                        {
                            existingDetail.Quantity = detail.Quantity;
                            existingDetail.Color = detail.Color;
                            existingDetail.ProductCategoryId = detail.ProductCategoryId;
                            await _repository.UpdateCartDetails(existingDetail);
                        }
                    }
                    else
                    {
                        throw new NotFoundException($"Cart item not found.");
                    }
                }
            }
            else
            {
                throw new NotFoundException($"User with ID: {userId} don't have any product in cart yet.");
            }

            //safety set data again in redis if not found in redis
            var result = await _repository.GetCart(userId, cancellationToken);

            return new UpdateCartResult(new BaseResponse<CartDto>
            {
                IsSuccess = true,
                Message = "Cart successfully updated.",
                Result = result
            });
        }
    }
}
