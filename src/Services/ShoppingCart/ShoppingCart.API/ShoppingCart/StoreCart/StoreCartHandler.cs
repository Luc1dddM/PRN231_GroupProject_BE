namespace ShoppingCart.API.ShoppingCart.StoreCart
{
    public record StoreCartCommand(CartHeader CartHeader) : ICommand<StoreCartResult>;
    public record StoreCartResult(bool IsSuccess, string Message);

    public class StoreCartHandler(ICartRepository repository) : ICommandHandler<StoreCartCommand, StoreCartResult>
    {
        public async Task<StoreCartResult> Handle(StoreCartCommand command, CancellationToken cancellationToken)
        {
            try
            {
                string resMessage = "";

                //checkif the cart header already existing for a user
                var existingHeader = await repository.GetCartHeaderByUserId(command.CartHeader.CreatedBy, cancellationToken);

                if (existingHeader != null) 
                {
                    //update cart header
                    existingHeader.UpdatedDate = DateTime.Now;
                    existingHeader.UpdatedBy = command.CartHeader.CreatedBy;
                    await repository.UpdateCartHeader(existingHeader, cancellationToken);
                    resMessage = "Cart successfully updated.";
                }
                else
                {
                    //create new cart header
                    var newHeader = await repository.CreateCartHeader(command.CartHeader, cancellationToken);
                    existingHeader = newHeader;
                    resMessage = "Cart successfully added.";
                }

                foreach (var detail in command.CartHeader.CartDetails)
                {
                    var existingDetail = await repository.GetCartDetailByCartHeaderId_ProductId(existingHeader.CartHeaderId, detail.ProductId, cancellationToken);

                    if (existingDetail != null)
                    {
                        //update quantity
                        existingDetail.Quantity += detail.Quantity;
                        existingDetail.Color = detail.Color;
                        await repository.UpdateCartDetails(existingDetail);
                    }
                    else
                    {
                        detail.CartDetailId = Guid.NewGuid().ToString();
                        detail.CartHeaderId = existingHeader.CartHeaderId;
                        await repository.CreateCartDetails(detail, cancellationToken);
                    }
                }

                return new StoreCartResult(true, resMessage);
            }
            catch (Exception ex)
            {

                return new StoreCartResult(false, $"Error occurred: {ex.Message}");
            }
        }
    }
}
