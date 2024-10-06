using Catalog.API.Models;

namespace Catalog.API.Repository
{
    public interface IProductCategoryRepository
    {
        public void CreateProductCategories(string brand, string device, string color, string productId, int quantity, bool status, string user);
        public void AddColor(ProductCategory productCategory, string user);
        public void CreateProductCategories(ProductCategory productCategory, string brand, string device, string user);

        //Update
        public void UpdateColor(string brand, string productId, bool status, int quantity, string user);
        public void UpdateBrand(string brand, string productId, bool status, string user);
        public void UpdateDevice(string device, string productId, bool status, string user);

        //Disable
        public void DisableByCategory(string CategoryId, string user);
        public void EnableByCategory(string CategoryId, string user);
        public void DisableByProduct(string ProductId, string user);
        public void EnableByProduct(string ProductId, string user);


        public List<ProductCategory> GetProductCategoriesByProductID(string ProductId);
        public List<ProductCategory> GetProductCategoriesByCategoryID(string CategoryId);
        public ProductCategory GetProductCategoriesByCategoryAndProductID(string CategoryId, string ProductId);
    }
}
