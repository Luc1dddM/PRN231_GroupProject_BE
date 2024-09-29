namespace ShoppingCart.API.ShoppingCart.DeleteCart
{
    public record DeleteCartCommand(string cartDetailId) : ICommand<DeleteCartResult>;
    public record DeleteCartResult(bool IsSuccess);

    public class DeleteCartHandler(ICartRepository repository) : ICommandHandler<DeleteCartCommand, DeleteCartResult>
    {
        public async Task<DeleteCartResult> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
        {
            await repository.DeleteCartDetails(command.cartDetailId, cancellationToken);
            return new DeleteCartResult(true);
        }
    }
}
