namespace ShoppingCart.API.ShoppingCart.GetCart
{
    public record GetCartQuery(string userId) : IQuery<GetCartResult>;

    public record GetCartResult(CartDto Cart);

    public class GetCartQueryHandler : IQueryHandler<GetCartQuery, GetCartResult>
    {

        private readonly ICartRepository repository;
        public GetCartQueryHandler(ICartRepository _repository)
        {
            repository = _repository;
        }
        public async Task<GetCartResult> Handle(GetCartQuery query, CancellationToken cancellationToken)
        {
            var cart = await repository.GetCart(query.userId, cancellationToken);

            return new GetCartResult(cart);
        }
    }
}
