using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly MyDbContext _dbContext;
        private readonly IProductCategoryRepository _productCategoriesRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductRepository(MyDbContext Context,
            IProductCategoryRepository productCategorieRepository,
            ICategoryRepository categoryRepository)
        {
            _dbContext = Context;
            _productCategoriesRepository = productCategorieRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task Create(Product product, string user, CancellationToken cancellationToken = default)
        {
            product.ImageUrl = product.ImageUrl;
            product.CreateBy = user;
            product.UpdateBy = user;
            product.CreateDate = DateTime.Now;
            product.UpdateDate = DateTime.Now;


            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Disable(string productId, string user)
        {
            try
            {
                Product product = GetProductByID(productId).Result;
                product.UpdateBy = user;
                product.UpdateDate = DateTime.Now;
                product.Status = false;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Enable(string productId, string user)
        {
            try
            {
                Product product = GetProductByID(productId).Result;
                product.UpdateBy = user;
                product.UpdateDate = DateTime.Now;
                product.Status = true;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Product> GetAll()
        {
            try
            {
                return  _dbContext.Products.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Product>> GetAllActive(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.Products.Where(p => p.Status).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> GetProductByID(string productId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId.Equals(productId), cancellationToken);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductDetailDTO> GetProductDetailById(string productId, CancellationToken cancellationToken = default)
        {
            try
            {

                var ProductRaw = await _dbContext.Products.FindAsync(productId, cancellationToken);
                var Brand =  _dbContext.ProductCategories.Include(c => c.Category).FirstOrDefault(p => p.Category.Type.Equals("Brand") && p.ProductId.Equals(productId));
                var Device =  _dbContext.ProductCategories.Include(c => c.Category).FirstOrDefault(p => p.Category.Type.Equals("Device") && p.ProductId.Equals(productId));
                var Color =  _dbContext.ProductCategories.Where(p => p.Category.Type.Equals("Color") && p.ProductId.Equals(productId)).ToList();

                var productDetailDTO = new ProductDetailDTO
                {
                    product = new Product
                    {
                        ProductId = ProductRaw.ProductId,
                        Name = ProductRaw.Name,
                        Price = ProductRaw.Price,
                        Description = ProductRaw.Description,
                        ImageUrl = ProductRaw.ImageUrl,
                        CreateBy = ProductRaw.CreateBy,
                        CreateDate = ProductRaw.CreateDate,
                        UpdateBy = ProductRaw.UpdateBy,
                        UpdateDate = ProductRaw.UpdateDate,
                        Status = ProductRaw.Status
                    },
                    brand = Brand.Adapt<ProductCategoryDTO>(),
                    device = Device.Adapt<ProductCategoryDTO>(),
                    color = Color.Adapt<List<ProductCategoryDTO>>(),
                };
                return productDetailDTO;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

/*        public ProductListDTO GetListCustomer(string[] colorParam, string[] brandParam, string[] deviceParam, string Price1, string Price2, string searchterm, int pageNumberParam, int pageSizeParam)
        {
            //Get List from db
            var result = _dbContext.Products.Include(p => p.ProductCategories).ThenInclude(p => p.Category).Where(p => p.Status && p.ProductCategories.Any(pc => pc.Category.Type.Equals("Color") && pc.Status)).ToList();

            //Call filter function 
            result = Filter(colorParam, brandParam, deviceParam, Price1, Price2, result);
            result = Search(result, searchterm);


            //Calculate pagination
            var totalItems = result.Count();
            var TotalPages = (int)Math.Ceiling((double)totalItems / pageSizeParam);

            //Get final result base on page size and page number 
            result = result.OrderByDescending(e => e.UpdateDate)
                    .Skip((pageNumberParam - 1) * pageSizeParam)
                    .Take(pageSizeParam)
                    .ToList();

            return new ProductListDTO()
            {
                listProduct = result,
                totalPages = TotalPages
            };
        }

        public ProductListDTO GetList(string[] colorParam, string[] brandParam, string[] deviceParam, string Price1, string Price2, string searchterm, int pageNumberParam, int pageSizeParam)
        {
            //Get List from db
            var result = _dbContext.Products.Include(p => p.ProductCategories).ToList();

            //Call filter function 
            result = Filter(colorParam, brandParam, deviceParam, Price1, Price2, result);
            result = Search(result, searchterm);

            //Calculate pagination
            var totalItems = result.Count();
            var TotalPages = (int)Math.Ceiling((double)totalItems / pageSizeParam);

            //Get final result base on page size and page number 
            result = result.OrderByDescending(e => e.UpdateDate)
                    .Skip((pageNumberParam - 1) * pageSizeParam)
                    .Take(pageSizeParam)
                    .ToList();

            return new ProductListDTO()
            {
                listProduct = result,
                totalPages = TotalPages
            };
        }*/

        private List<Product> Filter(string[] colorParam, string[] brand, string[] device, string Price1, string Price2, List<Product> list)
        {
            if (brand != null && brand.Length > 0)
            {
                list = list.Where(e => e.ProductCategories.Any(p => brand.Any(b => b.Equals(p.CategoryId)))).ToList();
            }

            if (device != null && device.Length > 0)
            {
                list = list.Where(e => e.ProductCategories.Any(p => device.Any(b => b.Equals(p.CategoryId)))).ToList();

            }

            if (colorParam != null && colorParam.Length > 0)
            {
                list = list.Where(e => e.ProductCategories.Any(p => colorParam.Any(b => b.Equals(p.CategoryId)))).ToList();

            }
            if (!string.IsNullOrEmpty(Price1) && string.IsNullOrEmpty(Price2) && double.Parse(Price1) > 0)
            {
                list = list.Where(e => e.Price >= double.Parse(Price1)).ToList();
            }
            if (!string.IsNullOrEmpty(Price2) && !string.IsNullOrEmpty(Price1) && double.Parse(Price2) > 0 && double.Parse(Price2) > double.Parse(Price1) && double.Parse(Price1) > 0)
            {
                list = list.Where(e => e.Price >= double.Parse(Price1) && e.Price <= double.Parse(Price2)).ToList();
            }
            if (!string.IsNullOrEmpty(Price2) && string.IsNullOrEmpty(Price1) && double.Parse(Price2) > 0)
            {
                list = list.Where(e => e.Price <= double.Parse(Price2)).ToList();
            }

            return list;
        }

        private List<Product> Search(List<Product> list, string searchtearm)
        {
            if (!string.IsNullOrEmpty(searchtearm))
            {
                list = list.Where(p =>
                            p.Name.Contains(searchtearm, StringComparison.OrdinalIgnoreCase))
                            .ToList();
            }
            return list;
        }



        public async Task Update(Product product, string user, CancellationToken cancellationToken = default)
        {
            try
            {
                Product newProduct = GetProductByID(product.ProductId).Result;
                newProduct.ProductId = product.ProductId;
                newProduct.Name = product.Name;
                newProduct.Description = product.Description;
                newProduct.Price = product.Price;
                newProduct.ImageUrl = product.ImageUrl;
                newProduct.Status = product.Status;
                newProduct.UpdateBy = user;
                newProduct.UpdateDate = DateTime.Now;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*public async Task ImportProducts(IFormFile excelFile, string user)
        {
            try
            {
                var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\uploads\\";

                var filePath = Path.Combine(uploadsFolder, excelFile.Name);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await excelFile.CopyToAsync(stream);
                }



                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            bool isHeaderSkipped = false;
                            while (reader.Read())
                            {
                                if (!isHeaderSkipped)
                                {
                                    isHeaderSkipped = true;
                                    continue;
                                }

                                Product s = new Product()
                                {
                                    Name = reader.GetValue(0).ToString() ?? "Error Name!",
                                    Price = double.Parse(reader.GetValue(1).ToString() ?? "0"),
                                    Description = reader.GetValue(2).ToString() ?? string.Empty,
                                    ImageUrl = reader.GetValue(3).ToString() ?? string.Empty,
                                    Status = bool.Parse(reader.GetValue(4).ToString() ?? "False"),
                                };
                                Create(s, user);
                                var brand = reader.GetValue(5).ToString() ?? "Error Brand!";
                                var device = reader.GetValue(6).ToString() ?? "Error Device!";
                                var color = reader.GetValue(7).ToString() ?? "Error Color!";
                                var quantity = int.Parse(reader.GetValue(8).ToString() ?? "0");
                                _productCategoriesRepository.CreateProductCategories(_categoryRepository.GetCategoryByName(brand).CategoryId, _categoryRepository.GetCategoryByName(device).CategoryId, _categoryRepository.GetCategoryByName(color).CategoryId, s.ProductId, quantity, s.Status, user);


                            }
                        } while (reader.NextResult());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }*/

        /*public async Task<byte[]> ExportProductsFilter(string[] colorParam, string[] brandParam, string[] deviceParam, string Price1, string Price2, string searchterm, int pageNumberParam, int pageSizeParam)
        {
            try
            {
                //Get List from db
                var result = await _dbContext.Products.Include(p => p.ProductCategories).ToListAsync();

                //Call filter function 
                result = Filter(colorParam, brandParam, deviceParam, Price1, Price2, result);
                result = Search(result, searchterm);

                DataTable dt = new DataTable();
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Price", typeof(double));
                dt.Columns.Add("Description", typeof(string));
                dt.Columns.Add("Image Base64", typeof(string));
                dt.Columns.Add("Status", typeof(bool));
                dt.Columns.Add("Brand", typeof(string));
                dt.Columns.Add("Device", typeof(string));
                dt.Columns.Add("Color", typeof(string));
                dt.Columns.Add("Quantity", typeof(int));
                dt.Columns.Add("Created By", typeof(string));
                dt.Columns.Add("Created Date", typeof(string));
                dt.Columns.Add("Updated By", typeof(string));
                dt.Columns.Add("Updated Date", typeof(string));

                foreach (var product in result)
                {
                    foreach (var item in _categoryRepository.GetChoosedColors(product))
                    {
                        DataRow row = dt.NewRow();
                        row[0] = product.Name;
                        row[1] = product.Price;
                        row[2] = product.Description;
                        row[3] = product.ImageUrl;
                        row[4] = product.Status;

                        row[5] = _categoryRepository.GetBrandsByProduct(product).Name;
                        row[6] = _categoryRepository.GetDevicesByProduct(product).Name;
                        row[7] = item.Name;
                        row[8] = _productCategoriesRepository.GetProductCategoriesByCategoryAndProductID(item.CategoryId, product.ProductId).Quantity;

                        row[9] = await _userRepo.GetUserNameById(item.CreatedBy);
                        row[10] = item.CreatedAt;
                        row[11] = await _userRepo.GetUserNameById(item.UpdateBy);
                        row[12] = item.UpdateDate;
                        dt.Rows.Add(row);
                    }

                }

                var memory = new MemoryStream();
                using (var excel = new ExcelPackage(memory))
                {
                    var worksheet = excel.Workbook.Worksheets.Add("Sheet1");

                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                    worksheet.DefaultRowHeight = 25;


                    return excel.GetAsByteArray();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }*/
    }
}
