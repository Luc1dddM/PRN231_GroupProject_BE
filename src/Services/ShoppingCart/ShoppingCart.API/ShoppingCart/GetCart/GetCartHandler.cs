using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ShoppingCart.API.ShoppingCart.GetCart
{
    public record GetCartQuery() : IQuery<GetCartResult>;

    public record GetCartResult(BaseResponse<CartDto> Result);

    public class GetCartQueryHandler : IQueryHandler<GetCartQuery, GetCartResult>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICartRepository repository;
        public GetCartQueryHandler(ICartRepository _repository, IHttpContextAccessor contextAccessor)
        {
            repository = _repository;
            _contextAccessor = contextAccessor;
        }
        public async Task<GetCartResult> Handle(GetCartQuery query, CancellationToken cancellationToken)
        {
            // get userId from HTTP context
            var userId = _contextAccessor.HttpContext?.Request.Headers["UserId"].ToString();

            if (string.IsNullOrEmpty(userId))
            {
                throw new NotFoundException("UserId did not have any value in the incoming request.");
            }


            var cart = await repository.GetCart(userId, cancellationToken);

            if (cart == null)
            {
                throw new NotFoundException($"Cart of user with ID {userId} did not exist.");
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
