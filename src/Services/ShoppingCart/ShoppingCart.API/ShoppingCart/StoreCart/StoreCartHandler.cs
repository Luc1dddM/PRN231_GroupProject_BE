namespace ShoppingCart.API.ShoppingCart.StoreCart
{
    public record StoreCartCommand(CartHeader CartHeader) : ICommand<StoreCartResult>;
    public record StoreCartResult(BaseResponse<CartDto> Result);

    public class StoreCartHandler : ICommandHandler<StoreCartCommand, StoreCartResult>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICartRepository _repository;
        public StoreCartHandler(ICartRepository repository, IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _contextAccessor = contextAccessor;
        }
        public async Task<StoreCartResult> Handle(StoreCartCommand command, CancellationToken cancellationToken)
        {

            var userId = _contextAccessor.HttpContext.Request.Headers["UserId"].ToString();

            if (string.IsNullOrEmpty(userId))
            {
                throw new NotFoundException("UserId did not have any value in the incoming request.");
            }

            string resMessage = "";

            //checkif the cart header already existing for a user
            var existingHeader = await _repository.GetCartHeaderByUserId(userId, cancellationToken);

            if (existingHeader != null)
            {
                //update cart header
                existingHeader.UpdatedDate = DateTime.Now;
                existingHeader.UpdatedBy = userId;
                await _repository.UpdateCartHeader(existingHeader, userId, cancellationToken);
                resMessage = "Cart successfully updated.";
            }
            else
            {
                //create new cart header
                var newHeader = await _repository.CreateCartHeader(userId, cancellationToken);
                existingHeader = newHeader;
                resMessage = "Cart successfully added.";
            }

            foreach (var detail in command.CartHeader.CartDetails)
            {
                var existingDetail = await _repository.GetCartDetailByCartHeaderId_ProductId(existingHeader.CartHeaderId, detail.ProductId, cancellationToken);

                if (existingDetail != null)
                {
                    //update quantity
                    existingDetail.Quantity += detail.Quantity;
                    existingDetail.Color = detail.Color;
                    existingDetail.ProductCategoryId = detail.ProductCategoryId;
                    await _repository.UpdateCartDetails(existingDetail);
                }
                else
                {
                    detail.CartDetailId = Guid.NewGuid().ToString();
                    detail.CartHeaderId = existingHeader.CartHeaderId;
                    await _repository.CreateCartDetails(detail, cancellationToken);
                }
            }

            var result = await _repository.GetCart(userId, cancellationToken);
            result.CartHeader.TotalPrice = result.CartDetails.Sum(c => c.Price * c.Quantity);

            return new StoreCartResult(new BaseResponse<CartDto>
            {
                IsSuccess = true,
                Message = resMessage,
                Result = result
            });

        }
    }
}
