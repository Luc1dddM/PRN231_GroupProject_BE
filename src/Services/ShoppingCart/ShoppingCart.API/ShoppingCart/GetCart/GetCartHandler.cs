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
            try
            {
                var cart = await repository.GetCart(query.userId, cancellationToken);

                return new GetCartResult(new BaseResponse<CartDto>
                {
                    IsSuccess = true,
                    Message = "Retrieve Cart Information Successful.",
                    Result = cart
                });
            }
            catch (Exception e)
            {

                return new GetCartResult(new BaseResponse<CartDto>
                {
                    IsSuccess = false,
                    Message = e.Message,
                });
            }

        }
    }
}
