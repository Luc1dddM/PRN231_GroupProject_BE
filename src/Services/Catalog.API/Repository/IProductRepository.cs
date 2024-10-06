using Catalog.API.Models;
using Catalog.API.Models.DTO;

namespace Catalog.API.Repository
{
    public interface IProductRepository
    {
        public Task Create(Product product, string user, CancellationToken cancellationToken=default);
        public Task Update(Product product, string user, CancellationToken cancellationToken = default);
       /* public Task ImportProducts(IFormFile excelFile, string user);
        public Task<Byte[]> ExportProductsFilter(string[] colorParam, string[] brandParam, string[] deviceParam, string Price1, string Price2, string searchterm, int pageNumberParam, int pageSizeParam);*/
        public void Disable(string productId, string user);
        public void Enable(string productId, string user);
        public List<Product> GetAll();
        public Task<List<Product>> GetAllActive(CancellationToken cancellationToken = default);

        public Task<Product> GetProductByID(string productId, CancellationToken cancellationToken = default);
        public Task<ProductDetailDTO> GetProductDetailById(string productId, CancellationToken cancellationToken = default);
/*        public ProductListDTO GetList(string[] colorParam, string[] brandParam, string[] deviceParam, string Price1, string Price2, string searchterm, int pageNumberParam, int pageSizeParam);
        public ProductListDTO GetListCustomer(string[] colorParam, string[] brandParam, string[] deviceParam, string Price1, string Price2, string searchterm, int pageNumberParam, int pageSizeParam);*/
    }
}
