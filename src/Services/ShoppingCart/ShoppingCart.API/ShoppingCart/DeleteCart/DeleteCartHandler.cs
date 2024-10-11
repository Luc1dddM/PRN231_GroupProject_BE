namespace ShoppingCart.API.ShoppingCart.DeleteCart
{
    public record DeleteCartCommand(string cartDetailId) : ICommand<DeleteCartResult>;
    public record DeleteCartResult(BaseResponse<object> Result);

    public class DeleteCartHandler : ICommandHandler<DeleteCartCommand, DeleteCartResult>
    {

        private readonly ICartRepository repository;
        public DeleteCartHandler(ICartRepository _repository)
        {
            repository = _repository;
        }
        public async Task<DeleteCartResult> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await repository.DeleteCart(command.cartDetailId, cancellationToken);
                return new DeleteCartResult(new BaseResponse<object>
                {
                    IsSuccess = true,
                    Message = "Your Cart Is Deleted."
                });
            }
            catch (Exception e)
            {
                return new DeleteCartResult(new BaseResponse<object>
                {
                    IsSuccess = false,
                    Message = e.Message
                });
            }

        }
    }
}
