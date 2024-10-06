using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Catalog.API.Repository;

namespace Catalog.API.Catagories.GetColor
{
    public record GetColorsQuery(string productId) : IQuery<GetColorsResult>;
    public record GetColorsResult(IEnumerable<Category> Categories);

    internal class GetColorsQueryHandler : IQueryHandler<GetColorsQuery, GetColorsResult>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public GetColorsQueryHandler(IHttpContextAccessor contextAccessor, ICategoryRepository categoryRepository)
        {
            _contextAccessor = contextAccessor;
            _categoryRepository = categoryRepository;
        }
        public async Task<GetColorsResult> Handle(GetColorsQuery query, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetColors(query.productId,cancellationToken);

            return new GetColorsResult(categories);
        }
    }
}
