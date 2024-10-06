using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Catalog.API.Repository;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery() : IQuery<GetProductsResult>;
    public record GetProductsResult(IEnumerable<Product> Products);

    internal class GetProductsQueryHandler: IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public GetProductsQueryHandler(IHttpContextAccessor contextAccessor, IProductRepository productRepository)
        {
            _contextAccessor = contextAccessor;
            _productRepository = productRepository;
        }
        public async  Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var products = _productRepository.GetAll();

            return new GetProductsResult(products);
        }
    }

}
