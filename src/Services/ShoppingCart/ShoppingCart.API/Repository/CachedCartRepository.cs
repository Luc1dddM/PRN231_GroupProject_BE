//the following using statement only for this class
using Microsoft.Extensions.Caching.Distributed;
using ShoppingCart.API.Models;
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
            try
            {
                //cache GetString to get the data value from Redis distributed cache
                //with userId as key
                var cacheKey = $"cart:{userId}";
                var cachedCartJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedCartJson))
                {
                    //if cart existed, deserialize to CartDto and return to GetCart method
                    return JsonSerializer.Deserialize<CartDto>(cachedCartJson, _options)!;
                }

                //if no cart exist, query in db
                var cart = await _repository.GetCart(userId, cancellationToken);
                if (cart is null)
                {
                    throw new CartNotFoundException(userId);
                }

                //and set key in redis distributed cache
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cart), cancellationToken);

                return cart;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
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

        public async Task<CartDetail> GetCartDetailByCartHeaderId_ProductId(string cartHeaderId, string productId, CancellationToken cancellationToken = default)
        {
            try
            {
                //we don't cache onl;y cart detail, as we want to keep the full cart object in cache
                var cartDetail = await _repository.GetCartDetailByCartHeaderId_ProductId(cartHeaderId, productId, cancellationToken);
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

                await _cache.SetStringAsync($"cart:{userId}", JsonSerializer.Serialize(cartDto), cancellationToken);

                return createdCartHeader;
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

                var cacheKey = $"cart:{cartDetail.CartHeader.CreatedBy}";

                var cachedCartJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedCartJson))
                {
                    var cartDto = JsonSerializer.Deserialize<CartDto>(cachedCartJson);

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

                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cartDto), cancellationToken);
                    }
                }

                return createdDetail;
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

                var cacheKey = $"cart:{cartDetail.CartHeader.CreatedBy}";

                var cachedCartJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedCartJson))
                {
                    var cartDto = JsonSerializer.Deserialize<CartDto>(cachedCartJson);

                    if (cartDto != null)
                    {
                        var cDetailToUpdate = cartDto.CartDetails.FirstOrDefault(cd => cd.ProductId == cartDetail.ProductId);
                        if (cDetailToUpdate != null)
                        {
                            cDetailToUpdate.ProductCategoryId = cartDetail.ProductCategoryId;
                            cDetailToUpdate.Quantity = cartDetail.Quantity;
                            cDetailToUpdate.Color = cartDetail.Color;
                        }
                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cartDto), cancellationToken);
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
                if (cartDetailToDelete == null)
                {
                    return false; //cart detail not found
                }

                string userId = cartDetailToDelete.CartHeader.CreatedBy;

                var deletedFromDb = await _repository.DeleteCartDetails(cartDetailId, cancellationToken);
                if (!deletedFromDb)
                {
                    return false;
                }

                var cacheKey = $"cart:{userId}";
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
                var cacheKey = $"cart:{cartToDelete.CreatedBy}";

                if (cartToDelete == null)
                {
                    return false;
                }

                await _repository.DeleteCart(userId, cancellationToken);

                await _cache.RemoveAsync(cacheKey, cancellationToken);

                return true;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }


    }
}
