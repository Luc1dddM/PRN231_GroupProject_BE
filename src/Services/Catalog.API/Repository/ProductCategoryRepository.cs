using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repository
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly MyDbContext _dbContext;
        private readonly ICategoryRepository _categoryRepository;
        public ProductCategoryRepository(MyDbContext Context, ICategoryRepository categoryRepository)
        {
            _dbContext = Context;
            _categoryRepository = categoryRepository;
        }

        public void CreateProductCategories(string brand, string device, string color, string productId, int quantity, bool status, string user)
        {
            try
            {

                var productCategory = new ProductCategory()
                {
                    CategoryId = brand,
                    ProductId = productId,
                    Quantity = 0,
                    CreatedBy = user,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Updatedby = user,
                    Status = status
                };
                _dbContext.ProductCategories.Add(productCategory);
                _dbContext.SaveChanges();

                var __productCategory = new ProductCategory()
                {
                    CategoryId = device,
                    ProductId = productId,
                    Quantity = 0,
                    CreatedBy = user,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Updatedby = user,
                    Status = status
                };
                _dbContext.ProductCategories.Add(__productCategory);
                _dbContext.SaveChanges();

                var _productCategory = new ProductCategory()
                {
                    CategoryId = color,
                    ProductId = productId,
                    Quantity = quantity,
                    CreatedBy = user,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Updatedby = user,
                    Status = status
                };
                _dbContext.ProductCategories.Add(_productCategory);
                _dbContext.SaveChanges();


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CreateProductCategories(ProductCategory productCategory, string brand, string device, string user)
        {
            try
            {
                if (brand != null)
                {
                    var ProductCategory = new ProductCategory()
                    {
                        CategoryId = brand,
                        ProductId = productCategory.ProductId,
                        Quantity = 0,
                        CreatedBy = user,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Updatedby = user,
                        Status = true
                    };
                    _dbContext.ProductCategories.Add(ProductCategory);
                    _dbContext.SaveChanges();
                }
                if (device != null)
                {
                    var ProductCategory = new ProductCategory()
                    {
                        CategoryId = device,
                        ProductId = productCategory.ProductId,
                        Quantity = 0,
                        CreatedBy = user,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Updatedby = user,
                        Status = true
                    };
                    _dbContext.ProductCategories.Add(ProductCategory);
                    _dbContext.SaveChanges();
                }



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddColor(ProductCategory productCategory, string user)
        {
            try
            {

                productCategory.CreatedBy = user;
                productCategory.CreatedAt = DateTime.Now;
                productCategory.Updatedby = user;
                productCategory.UpdatedAt = DateTime.Now;


                _dbContext.ProductCategories.Add(productCategory);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteProductCategory(string ProductId, string CategoryId)
        {
            try
            {
                var productCategory = _dbContext.ProductCategories.FirstOrDefault(c => c.ProductId.Equals(ProductId) && c.CategoryId.Equals(CategoryId));
                if (productCategory != null)
                {
                    _dbContext.ProductCategories.Remove(productCategory);
                    _dbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DisableByCategory(string CategoryId, string user)
        {
            try
            {
                List<ProductCategory> productCategories = GetProductCategoriesByCategoryID(CategoryId);
                if (productCategories.Count != 0)
                {
                    foreach (var productCategory in productCategories)
                    {
                        productCategory.Updatedby = user;
                        productCategory.UpdatedAt = DateTime.Now;
                        productCategory.Status = false;
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DisableByProduct(string ProductId, string user)
        {
            try
            {
                List<ProductCategory> productCategories = GetProductCategoriesByProductID(ProductId);
                if (productCategories.Count != 0)
                {
                    foreach (var productCategory in productCategories)
                    {
                        productCategory.Updatedby = user;
                        productCategory.UpdatedAt = DateTime.Now;
                        productCategory.Status = false;
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void EnableByCategory(string CategoryId, string user)
        {
            try
            {
                List<ProductCategory> productCategories = GetProductCategoriesByCategoryID(CategoryId);
                if (productCategories.Count != 0)
                {
                    foreach (var productCategory in productCategories)
                    {
                        productCategory.Updatedby = user;
                        productCategory.UpdatedAt = DateTime.Now;
                        productCategory.Status = true;
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void EnableByProduct(string ProductId, string user)
        {
            {
                try
                {
                    List<ProductCategory> productCategories = GetProductCategoriesByProductID(ProductId);
                    if (productCategories.Count != 0)
                    {
                        foreach (var productCategory in productCategories)
                        {
                            productCategory.Updatedby = user;
                            productCategory.UpdatedAt = DateTime.Now;
                            productCategory.Status = true;
                            _dbContext.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public ProductCategory GetProductCategoriesByCategoryAndProductID(string CategoryId, string ProductId)
        {
            try
            {
                return _dbContext.ProductCategories.FirstOrDefault(c => c.CategoryId.Equals(CategoryId) && c.ProductId.Equals(ProductId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ProductCategory> GetProductCategoriesByCategoryID(string CategoryId)
        {
            try
            {
                return _dbContext.ProductCategories.Where(c => c.CategoryId.Equals(CategoryId)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ProductCategory> GetProductCategoriesByProductID(string ProductId)
        {
            try
            {
                return _dbContext.ProductCategories.Where(c => c.ProductId.Equals(ProductId) && c.Category.Type.Equals("Color")).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateProductCategories(ProductCategory productCategory, string user)
        {
            try
            {
                ProductCategory newProductCategory = GetProductCategoriesByCategoryAndProductID(productCategory.CategoryId, productCategory.ProductId);
                newProductCategory.Updatedby = user;
                newProductCategory.UpdatedAt = DateTime.Now;
                newProductCategory.Quantity = productCategory.Quantity;
                newProductCategory.Status = productCategory.Status;
                if (_categoryRepository.GetCategoryByID(productCategory.CategoryId).Type.Equals("Color") && productCategory.Quantity == 0)
                {
                    newProductCategory.Status = false;
                }
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateDevice(string device, string productId, bool status, string user)
        {
            try
            {
                ProductCategory newProductCategory = _dbContext.ProductCategories.Include(p => p.Category)
                                                        .FirstOrDefault(c => c.Category.Type.Equals("Device") && c.ProductId.Equals(productId));
                newProductCategory.CategoryId = device;
                newProductCategory.Updatedby = user;
                newProductCategory.UpdatedAt = DateTime.Now;
                newProductCategory.Quantity = 0;
                newProductCategory.Status = status;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateBrand(string brand, string productId, bool status, string user)
        {
            try
            {
                ProductCategory newProductCategory = _dbContext.ProductCategories.Include(p => p.Category)
                                                        .FirstOrDefault(c => c.Category.Type.Equals("Brand") && c.ProductId.Equals(productId));
                newProductCategory.CategoryId = brand;
                newProductCategory.Updatedby = user;
                newProductCategory.UpdatedAt = DateTime.Now;
                newProductCategory.Quantity = 0;
                newProductCategory.Status = status;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateColor(string color, string productId, bool status, int quantity, string user)
        {
            try
            {
                ProductCategory newProductCategory = _dbContext.ProductCategories.FirstOrDefault(c => c.CategoryId.Equals(color) && c.ProductId.Equals(productId));
                newProductCategory.Updatedby = user;
                newProductCategory.UpdatedAt = DateTime.Now;
                newProductCategory.Quantity = quantity;
                if (quantity != 0)
                {
                    newProductCategory.Status = status;
                }
                else
                {
                    newProductCategory.Status = false;
                }
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
