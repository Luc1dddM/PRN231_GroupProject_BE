using Catalog.API.Models;
using Catalog.API.Models.DTO;


namespace Catalog.API.Repository
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetAll(CancellationToken cancellationToken=default);
        public List<Category> GetAllActive();
        public Category GetCategoryByName(string name);
        public Task<List<Category>> GetColors(string producId, CancellationToken cancellationToken=default);
        public List<Category> GetBrands();
        public List<Category> GetDevices();
        public List<Category> GetColors();
        public List<Category> GetChoosedColors(Product Product);
        public Category GetCategoryByID(string categoryId);
        public Category GetDevicesByProduct(Product Product);
        public Category GetBrandsByProduct(Product Product);
        public List<Category> GetChoosedCategoriesByProduct(Product Product);
        /*public CategoryListDTO GetList(string[] statusesParam, string[] TypeParam, string searchterm, string sortBy, string sortOrder, int pageNumberParam, int pageSizeParam);*/

        public void Create(Category category, string user);
        /*public Task ImportCategories(IFormFile excelFile, string user);
        public Task<Byte[]> ExportCategoriesFilter(string[] statusesParam, string[] categoriesParam, string searchterm, int pageNumberParam, int pageSizeParam);*/

        public void update(Category category, string user);

        public bool haveDevice(Product Product);
        public bool haveBrand(Product Product);
    }
}
