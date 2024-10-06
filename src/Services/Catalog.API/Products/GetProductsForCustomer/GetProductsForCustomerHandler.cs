using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Catalog.API.Repository;

namespace Catalog.API.Products.GetProductsForCustomer
{
    public record GetProductsForCustomerQuery() : IQuery<GetProductsForCustomerResult>;
    public record GetProductsForCustomerResult(IEnumerable<Product> Products);

    internal class GetProductsQueryHandler : IQueryHandler<GetProductsForCustomerQuery, GetProductsForCustomerResult>
    {
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public GetProductsQueryHandler(IHttpContextAccessor contextAccessor, IProductRepository productRepository)
        {
            _contextAccessor = contextAccessor;
            _productRepository = productRepository;
        }
        public async Task<GetProductsForCustomerResult> Handle(GetProductsForCustomerQuery query, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllActive();

            return new GetProductsForCustomerResult(products);
        }
    }
}
