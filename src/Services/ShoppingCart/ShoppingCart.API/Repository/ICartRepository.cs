namespace ShoppingCart.API.Repository
{
    public interface ICartRepository
    {
        Task<CartDto> GetCart(string userId, CancellationToken cancellationToken = default);
        Task<CartHeader> GetCartById(string cartId, CancellationToken cancellationToken = default);
        Task<CartHeader> GetCartHeaderByUserId(string userId, CancellationToken cancellationToken = default);
        Task<CartDetail> GetCartDetailById(string cartDetailId, CancellationToken cancellationToken = default); 
        Task<CartDetail> GetCartDetailByCartHeaderId_ProductCategoryId(string cartHeaderId, string productCategoryId, CancellationToken cancellationToken = default!);
        Task<CartHeader> CreateCartHeader(string userId, CancellationToken cancellationToken = default);
        Task<CartDetail> CreateCartDetails(CartDetail cartDetail, CancellationToken cancellationToken = default!);
        Task<CartHeader> UpdateCartHeader(CartHeader cartHeader, string userId, CancellationToken cancellationToken = default!);
        Task<CartDetail> UpdateCartDetails(CartDetail cartDetail , CancellationToken cancellationToken = default!);
        Task<bool> DeleteCart(string userId, CancellationToken cancellationToken = default);
        Task<bool> DeleteCartDetails(string cartDetailId, CancellationToken cancellationToken = default!);
    }
}
