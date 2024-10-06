using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Catalog.API.Repository;

namespace Catalog.API.Categories.GetCategories
{
    public record GetCategoriesQuery() : IQuery<GetCategoriesResult>;
    public record GetCategoriesResult(IEnumerable<Category> Categories);

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
            var categories = await _categoryRepository.GetAll(cancellationToken);

            return new GetCategoriesResult(categories);
        }
    }

}
