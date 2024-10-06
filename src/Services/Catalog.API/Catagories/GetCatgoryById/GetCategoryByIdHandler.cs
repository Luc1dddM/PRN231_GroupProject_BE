using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;

namespace Catalog.API.Categories.GetCategoryById
{
    public record GetCategoryByIdQuery(string Id) : IQuery<GetCategoryByIdResult>;
    public record GetCategoryByIdResult(Category Category);

    internal class GetCategoryByIdQueryHandler
        : IQueryHandler<GetCategoryByIdQuery, GetCategoryByIdResult>
    {

        private readonly ICategoryRepository _categoryRepository;
        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<GetCategoryByIdResult> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            var category = _categoryRepository.GetCategoryByID(query.Id);

            if (category is null)
            {
                throw new ProductNotFoundException(query.Id);
            }

            return new GetCategoryByIdResult(category);
        }
    }
}