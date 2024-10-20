using ShoppingCart.API.Models;

namespace ShoppingCart.API.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDBContext _context;
        public CartRepository(CartDBContext context)
        {
            _context = context;
        }

        public async Task<CartDto> GetCart(string userId, CancellationToken cancellationToken = default)
        {

            //this method purpose serve as a wiat to get all the data of Cart Header including CartDetail since map from GetCartResult to GetCartResponse won't work
            var cart = await _context.CartHeaders.Include(ch => ch.CartDetails)
                                                 .FirstOrDefaultAsync(c => c.CreatedBy.Equals(userId), cancellationToken);
            if (cart == null)
            {
                throw new NotFoundException($"Cart of user with ID {userId} did not exist.");
            }
            else
            {
                return new CartDto
                {
                    CartHeader = new CartHeaderDto
                    {
                        CartHeaderId = cart.CartHeaderId,
                        TotalPrice = cart.TotalPrice,
                    },
                    CartDetails = cart.CartDetails.Select(cd => new CartDetailDto
                    {
                        CartDetailId = cd.CartDetailId,
                        ProductId = cd.ProductId,
                        ProductName = cd.ProductName,
                        Quantity = cd.Quantity,
                        Color = cd.Color,
                        Price = cd.Price,
                        ProductCategoryId = cd.ProductCategoryId,
                        CartHeaderId = cart.CartHeaderId
                    }).ToList()
                };
            }
        }

        public async Task<CartHeader> GetCartById(string cartId, CancellationToken cancellationToken = default)
        {
            try
            {
                //get cart header and it correspond cart detail 
                var cHeader = await _context.CartHeaders.Include(ch => ch.CartDetails)
                                                        .FirstOrDefaultAsync(c => c.CartHeaderId.Equals(cartId), cancellationToken);

                return cHeader;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<CartHeader> GetCartHeaderByUserId(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                //get cart header and it correspond cart detail 
                var cHeader = await _context.CartHeaders.Include(ch => ch.CartDetails)
                                                        .FirstOrDefaultAsync(c => c.CreatedBy.Equals(userId), cancellationToken);

                return cHeader;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<CartDetail> GetCartDetailByCartHeaderId_ProductCategoryId(string cartHeaderId, string productCategoryId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cDetail = await _context.CartDetails.Include(cd => cd.CartHeader)
                                                                    .FirstOrDefaultAsync(c => c.CartHeaderId.Equals(cartHeaderId) &&
                                                                                              c.ProductCategoryId.Equals(productCategoryId), cancellationToken);
                return cDetail;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<CartDetail> GetCartDetailById(string cartDetailId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cartDetail = await _context.CartDetails.Include(cd => cd.CartHeader).FirstOrDefaultAsync(cd => cd.CartDetailId.Equals(cartDetailId));

                if (cartDetail == null)
                {
                    throw new NotFoundException($"CartDetail with Id {cartDetailId} was not found");
                }

                return cartDetail;
            }
            catch (NotFoundException e)
            {
                throw new NotFoundException($"CartDetail with Id {cartDetailId} was not found");
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<CartHeader> CreateCartHeader(string userId, CancellationToken cancellationToken = default)
        {

            try
            {
                if (userId == null)
                {
                    throw new NotFoundException("UserId did not have any value in the incoming request.");
                }
                CartHeader cHeader = new()
                {
                    CartHeaderId = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedBy = userId,
                };
                _context.CartHeaders.Add(cHeader);
                await _context.SaveChangesAsync(cancellationToken);
                return cHeader;
            }
            catch (NotFoundException e)
            {
                throw new NotFoundException(e.Message);
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<CartDetail> CreateCartDetails(CartDetail cartDetail, CancellationToken cancellationToken = default)
        {

            try
            {
                if (cartDetail.ProductId == null ||
                cartDetail.ProductCategoryId == null)
                {
                    throw new NotFoundException("CartDetail contains invalid or missing data.");
                }
                _context.CartDetails.Add(cartDetail);
                await _context.SaveChangesAsync(cancellationToken);
                return cartDetail;
            }
            catch (NotFoundException e)
            {
                throw new NotFoundException("CartDetail contains invalid or missing data.");
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<CartHeader> UpdateCartHeader(CartHeader cartHeader, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.CartHeaders.Update(cartHeader);
                await _context.SaveChangesAsync(cancellationToken);
                return cartHeader;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<CartDetail> UpdateCartDetails(CartDetail cartDetail, CancellationToken cancellationToken = default)
        {
            if (cartDetail.ProductId == null ||
                cartDetail.ProductCategoryId == null)
            {
                throw new NotFoundException("CartDetail contains invalid or missing data.");
            }
            try
            {
                _context.CartDetails.Update(cartDetail);
                await _context.SaveChangesAsync(cancellationToken);
                return cartDetail;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<bool> DeleteCartDetails(string cartDetailId, CancellationToken cancellationToken = default)
        {

            try
            {
                var cartDetail = await GetCartDetailById(cartDetailId, cancellationToken);
                if (cartDetail == null)
                {
                    throw new NotFoundException($"CartDetail with Id {cartDetailId} was not found");
                }
                _context.CartDetails.Remove(cartDetail);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (NotFoundException e)
            {
                throw new NotFoundException($"CartDetail with Id {cartDetailId} was not found");
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }



        public async Task<bool> DeleteCart(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cartHeader = await GetCartHeaderByUserId(userId, cancellationToken);
                if (cartHeader == null)
                {
                    throw new NotFoundException($"Cart of user with Id {userId} was not found");
                }
                _context.CartHeaders.Remove(cartHeader);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (NotFoundException e)
            {
                throw new NotFoundException($"Cart of user with Id {userId} was not found");
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }
    }
}
