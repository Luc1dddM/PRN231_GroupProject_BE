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

            await repository.DeleteCartDetails(command.cartDetailId, cancellationToken);
            return new DeleteCartResult(new BaseResponse<object>
            {
                IsSuccess = true,
                Message = "Your Cart Item Is Deleted."
            });
        }
    }
}
