//the following using statement only for this class
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ShoppingCart.API.Repository
{
    public class CachedCartRepository : ICartRepository
    {
        private readonly ICartRepository _repository;
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _options;

        public CachedCartRepository(ICartRepository repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
            _options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                MaxDepth = 64
            };
        }


        public async Task<CartDto> GetCart(string userId, CancellationToken cancellationToken = default)
        {

            //cache GetString to get the data value from Redis distributed cache
            //with userId as key
            var cacheKey = $"cartOfUser:{userId}";
            var cachedCartJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedCartJson))
            {
                //if cart existed, deserialize to CartDto and return to GetCart method
                return JsonSerializer.Deserialize<CartDto>(cachedCartJson, _options)!;
            }

            //if no cart exist, query in db
            var cart = await _repository.GetCart(userId, cancellationToken);

            //and set key in redis distributed cache
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cart, _options), cancellationToken);

            return cart;

        }

        public async Task<CartHeader> GetCartById(string cartId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cart = await _repository.GetCartById(cartId, cancellationToken);
                if (cart is null)
                {
                    return null;
                }
                return cart;
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
                //we don't cache just the cart header, as we want to keep the full cart object in cache
                var cart = await _repository.GetCartHeaderByUserId(userId, cancellationToken);
                if (cart is null)
                {
                    return null;
                }
                return cart;
            }
            catch (Exception e) { throw new Exception(e.Message); }

        }

        public async Task<CartDetail> GetCartDetailByCartHeaderId_ProductCategoryId(string cartHeaderId, string productCategoryId, CancellationToken cancellationToken = default)
        {
            try
            {
                //we don't cache onl;y cart detail, as we want to keep the full cart object in cache
                var cartDetail = await _repository.GetCartDetailByCartHeaderId_ProductCategoryId(cartHeaderId, productCategoryId, cancellationToken);
                if (cartDetail is null)
                {
                    return null;
                }
                return cartDetail;
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
                var cartDetail = await _repository.GetCartDetailById(cartDetailId, cancellationToken);

                return cartDetail;
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
                //first create cart in database
                var createdCartHeader = await _repository.CreateCartHeader(userId, cancellationToken);

                var cartDto = new CartDto
                {
                    CartHeader = new CartHeaderDto
                    {
                        CartHeaderId = createdCartHeader.CartHeaderId,
                        TotalPrice = createdCartHeader.TotalPrice,
                    },
                    CartDetails = new List<CartDetailDto>()
                };

                await _cache.SetStringAsync($"cartOfUser:{userId}", JsonSerializer.Serialize(cartDto, _options), cancellationToken);

                return createdCartHeader;
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
                var createdDetail = await _repository.CreateCartDetails(cartDetail, cancellationToken);

                var cacheKey = $"cartOfUser:{cartDetail.CartHeader.CreatedBy}";

                var cachedCartJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedCartJson))
                {
                    var cartDto = JsonSerializer.Deserialize<CartDto>(cachedCartJson, _options);

                    if (cartDto != null)
                    {
                        cartDto.CartDetails.Add(new CartDetailDto
                        {
                            CartDetailId = createdDetail.CartDetailId,
                            ProductId = createdDetail.ProductId,
                            ProductName = createdDetail.ProductName,
                            Quantity = createdDetail.Quantity,
                            Color = createdDetail.Color,
                            Price = createdDetail.Price,
                            ProductCategoryId = createdDetail.ProductCategoryId,
                            CartHeaderId = createdDetail.CartHeaderId,
                        });

                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cartDto, _options), cancellationToken);
                    }
                }

                return createdDetail;
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

        public async Task<CartHeader> UpdateCartHeader(CartHeader cartHeader, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var updatedHeader = await _repository.UpdateCartHeader(cartHeader, userId, cancellationToken);

                return updatedHeader;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<CartDetail> UpdateCartDetails(CartDetail cartDetail, CancellationToken cancellationToken = default)
        {
            try
            {
                var updatedDetail = await _repository.UpdateCartDetails(cartDetail, cancellationToken);

                var cacheKey = $"cartOfUser:{cartDetail.CartHeader.CreatedBy}";

                var cachedCartJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedCartJson))
                {
                    var cartDto = JsonSerializer.Deserialize<CartDto>(cachedCartJson, _options);

                    if (cartDto != null)
                    {
                        var cDetailToUpdate = cartDto.CartDetails.FirstOrDefault(cd => cd.CartDetailId == cartDetail.CartDetailId);
                        if (cDetailToUpdate != null)
                        {
                            cDetailToUpdate.ProductCategoryId = cartDetail.ProductCategoryId;
                            cDetailToUpdate.Quantity = cartDetail.Quantity;
                            cDetailToUpdate.Color = cartDetail.Color;
                        }
                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cartDto, _options), cancellationToken);
                    }
                }

                return updatedDetail;
            }
            catch (Exception e) { throw new Exception(e.Message); }

        }

        public async Task<bool> DeleteCartDetails(string cartDetailId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cartDetailToDelete = await _repository.GetCartDetailById(cartDetailId, cancellationToken);

                string userId = cartDetailToDelete.CartHeader.CreatedBy;

                var deletedFromDb = await _repository.DeleteCartDetails(cartDetailId, cancellationToken);
                if (!deletedFromDb)
                {
                    return false;
                }

                var cacheKey = $"cartOfUser:{userId}";
                var cachedCartJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedCartJson))
                {
                    var cartDto = JsonSerializer.Deserialize<CartDto>(cachedCartJson, _options);
                    if (cartDto != null)
                    {
                        //find and remove the specific cart detail
                        var cartDetailToRemove = cartDto.CartDetails.FirstOrDefault(cd => cd.CartDetailId == cartDetailId);
                        if (cartDetailToRemove != null)
                        {
                            cartDto.CartDetails.Remove(cartDetailToRemove);

                            //store the updated CartDto back in Redis
                            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cartDto, _options), cancellationToken);
                        }
                    }
                }
                return true;
            }
            catch (Exception e) { throw new Exception(e.Message); }

        }

        public async Task<bool> DeleteCart(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cartToDelete = await GetCartHeaderByUserId(userId);

                if (cartToDelete == null)
                {
                    throw new NotFoundException($"Cart of user with Id {userId} is not found.");
                }

                var cacheKey = $"cartOfUser:{cartToDelete.CreatedBy}";

                await _repository.DeleteCart(userId, cancellationToken);

                await _cache.RemoveAsync(cacheKey, cancellationToken);

                return true;
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


    }
}
