using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;

namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdQuery(string Id) : IQuery<GetProductByIdResult>;
    public record GetProductByIdResult(ProductDetailDTO Product);

    internal class GetProductByIdQueryHandler
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {

        private readonly IProductRepository _productRepository;
        public GetProductByIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductDetailById(query.Id, cancellationToken);

            if (product is null)
            {
                throw new ProductNotFoundException(query.Id);
            }

            return new GetProductByIdResult(product);
        }
    }
}