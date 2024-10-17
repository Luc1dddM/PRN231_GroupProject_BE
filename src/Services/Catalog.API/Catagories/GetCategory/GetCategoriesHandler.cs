using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;

namespace Catalog.API.Categories.GetCategories
{
    public record GetCategoriesQuery(GetListCategoryParamsDto getListCategoryParamsDto) : IQuery<GetCategoriesResult>;
    public record GetCategoriesResult(BaseResponse<PaginatedList<CategoryDTO>> Categories);

    internal class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, GetCategoriesResult>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public GetCategoriesQueryHandler(IHttpContextAccessor contextAccessor, ICategoryRepository categoryRepository)
        {
            _contextAccessor = contextAccessor;
            _categoryRepository = categoryRepository;
        }
        public async  Task<GetCategoriesResult> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetList(query.getListCategoryParamsDto);

            var listCategories = categories.Adapt<List<CategoryDTO>>();

            // Step 6: Apply pagination on the filtered employee list
            var list = await PaginatedList<CategoryDTO>.CreateAsync(listCategories.AsQueryable(), query.getListCategoryParamsDto.PageNumber, query.getListCategoryParamsDto.PageSize);


            var result = new BaseResponse<PaginatedList<CategoryDTO>>(list);
            return new GetCategoriesResult(result);
        }
    }

}
