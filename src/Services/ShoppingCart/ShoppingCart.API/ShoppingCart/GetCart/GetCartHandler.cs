namespace ShoppingCart.API.ShoppingCart.GetCart
{
    public record GetCartQuery(string userId) : IQuery<GetCartResult>;

    public record GetCartResult(BaseResponse<CartDto> Result);

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

            if (cart == null)
            {
                throw new NotFoundException($"Cart of user with ID {query.userId} did not exist.");
            }

            return new GetCartResult(new BaseResponse<CartDto>
            {
                IsSuccess = true,
                Message = "Retrieve Cart Information Successful.",
                Result = cart
            });

        }
    }
}
