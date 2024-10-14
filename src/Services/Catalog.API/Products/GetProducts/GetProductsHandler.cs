using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery(GetListProductParamsDto getListProductParamsDto) : IQuery<GetProductsResult>;
    public record GetProductsResult(BaseResponse<PaginatedList<ProductDTO>> Products);

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


            /*var roles = _contextAccessor.HttpContext.Request.Headers["Roles"].ToString();*/
            var roles = "Admin";
            if (string.IsNullOrEmpty(roles)) throw new BadRequestException("Role Is Null");


            List<ProductDTO> products = new List<ProductDTO>();
            if(roles == "Cutomer")
            {
                products = _productRepository.GetListCustomer(query.getListProductParamsDto);

            }
            else
            {
                products = _productRepository.GetList(query.getListProductParamsDto);

            }

            var list = await PaginatedList<ProductDTO>.CreateAsync(products.AsQueryable(), query.getListProductParamsDto.PageNumber, query.getListProductParamsDto.PageSize);


            var result = new BaseResponse<PaginatedList<ProductDTO>>(list);

            return new GetProductsResult(result);
        }
    }

}
