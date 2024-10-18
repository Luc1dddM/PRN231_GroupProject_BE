using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Catalog.API.Exceptions;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;

namespace Catalog.API.Products.GetProductDetail
{

    public record GetProductDetailForOrderQuery(string Id) : IQuery<GetProductDetailForOrderResult>;
    public record GetProductDetailForOrderResult(BaseResponse<ProductDetailForOrderDTO> Product);

    internal class GetProductDetailForOrderHandler
        : IQueryHandler<GetProductDetailForOrderQuery, GetProductDetailForOrderResult>
    {

        private readonly IProductRepository _productRepository;
        public GetProductDetailForOrderHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<GetProductDetailForOrderResult> Handle(GetProductDetailForOrderQuery query, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductDetailForOrder(query.Id, cancellationToken);

            if (product.product is null)
            {
                throw new ProductNotFoundException(query.Id);
            }

            var result = new BaseResponse<ProductDetailForOrderDTO>(product);

            return new GetProductDetailForOrderResult(result);
        }
    }
}
