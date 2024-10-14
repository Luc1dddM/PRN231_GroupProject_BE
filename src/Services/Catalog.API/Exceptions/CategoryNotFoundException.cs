using BuildingBlocks.Exceptions;

namespace Catalog.API.Exceptions
{
    public class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException(string categoryId) : base("Category", categoryId)
        {
        }
    }
}
