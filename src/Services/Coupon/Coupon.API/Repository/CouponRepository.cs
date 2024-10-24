

using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using BuildingBlocks.Validation;
using Coupon.API.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using ExcelDataReader;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Hosting;


namespace Coupon.API.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private IWebHostEnvironment environment;
        private readonly Prn231GroupProjectContext _dbContext;

        public CouponRepository(Prn231GroupProjectContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            environment = env;
        }

        public async Task<Models.Coupon> CreateCoupon(Models.Coupon coupon,string userId)
        {
            try
            {
                coupon.CreatedDate = DateTime.Now;
                coupon.CreatedBy = userId;
                _dbContext.Coupons.Add(coupon);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return coupon;
        }

        public async Task<IEnumerable<Models.Coupon>> GetAllCoupons()
        {
            return await _dbContext.Coupons.ToListAsync();
        }

        public async Task<PaginatedList<CouponListDTO>> GetAllCoupons(GetListCouponParamsDto parameters)
        {
            IQueryable<Models.Coupon> query = _dbContext.Coupons.AsQueryable();

            // Step 1: Apply filters (e.g., Status, MinAmount, MaxAmount)
            if (parameters.Statuse is not null || parameters.MinAmount.HasValue || parameters.MaxAmount.HasValue)
            {
                query = Filter(parameters.Statuse, parameters.MinAmount, parameters.MaxAmount, query, parameters.StartDate, parameters.EndDate);

            }

            // Step 2: Apply keyword search
            query = Search(query, parameters?.Keyword ?? "");

            // Step 3: Apply sorting
            query = Sort(parameters.SortBy, parameters.SortOrder, query);

            // Step 4: Fetch all coupons that meet the criteria
            var allCoupons = await query.AsNoTracking().ToListAsync();

            // Step 5: Convert to DTO
            var couponDtoList = allCoupons.Select(coupon => coupon.Adapt<CouponListDTO>()).ToList();

            // Step 6: Apply pagination on the filtered coupon list
            var paginatedCoupons = await PaginatedList<CouponListDTO>.CreateAsync(couponDtoList.AsQueryable(), parameters.PageNumber, parameters.PageSize);

            return paginatedCoupons;
        }


        public async Task<Models.Coupon?> UpdateCoupon(Models.Coupon coupon, string userId)
        {
            try
            {
                var existingCoupon = await _dbContext.Coupons.FindAsync(coupon.Id);

                if (existingCoupon == null)
                {
                    return null;
                }

                existingCoupon.CouponCode = coupon.CouponCode;
                existingCoupon.DiscountAmount = coupon.DiscountAmount;
                existingCoupon.Quantity = coupon.Quantity;
                existingCoupon.Status = coupon.Status;
                existingCoupon.MinAmount = coupon.MinAmount;
                existingCoupon.MaxAmount = coupon.MaxAmount;
                existingCoupon.UpdatedBy = userId;
                existingCoupon.UpdatedDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                await _dbContext.UpdateCouponStatusIfNeeded(existingCoupon.CouponCode);
                return existingCoupon;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the coupon.", ex);
            }
        }

        private IQueryable<Models.Coupon> Filter(string[] statuses, double? minAmount, double? maxAmount, IQueryable<Models.Coupon> list, DateTime? startDate, DateTime? endDate)
        {
            if (minAmount.HasValue)
            {
                list = list.Where(e => e.MinAmount.HasValue && e.MinAmount.Value >= minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                list = list.Where(e => e.MaxAmount.HasValue && e.MaxAmount.Value <= maxAmount.Value);
            }

            if (statuses != null && statuses.Length > 0)
            {
                list = list.Where(e => statuses.Contains(e.Status.ToString()));
            }

            if (statuses != null && statuses.Length > 0)
            {
                list = list.Where(e => statuses.Contains(e.Status.ToString()));
            }

            if (startDate.HasValue)
            {
                list = list.Where(e => e.CreatedDate >= startDate.Value);
            }


            if (endDate.HasValue)
            {

                endDate = endDate.Value.AddDays(1).AddTicks(-1);
                list = list.Where(e => e.CreatedDate <= endDate.Value);
            }

            return list;
        }


        private IQueryable<Models.Coupon> Search(IQueryable<Models.Coupon> list, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();

                // Kiểm tra nếu searchTerm có thể chuyển đổi thành double   
                if (double.TryParse(searchTerm, out var searchTermValue))
                {
                    list = list.Where(p =>
                        p.CouponCode.ToLower().Contains(searchTermLower) ||
                        p.DiscountAmount == searchTermValue ||
                        (p.MinAmount.HasValue && p.MinAmount.Value == searchTermValue) ||
                        (p.MaxAmount.HasValue && p.MaxAmount.Value == searchTermValue)
                    );
                }
                else
                {
                    list = list.Where(p =>
                        p.CouponCode.ToLower().Contains(searchTermLower) ||
                        (p.MinAmount.HasValue && p.MinAmount.Value.ToString().ToLower().Contains(searchTermLower)) ||
                        (p.MaxAmount.HasValue && p.MaxAmount.Value.ToString().ToLower().Contains(searchTermLower))
                    );
                }
            }
            return list;
        }


        private IQueryable<Models.Coupon> Sort(string sortBy, string sortOrder, IQueryable<Models.Coupon> list)
        {
            switch (sortBy)
            {
                case "couponCode":
                    list = sortOrder == "asc" ? list.OrderBy(e => e.CouponCode) : list.OrderByDescending(e => e.CouponCode);
                    break;
                case "discountAmount":
                    list = sortOrder == "asc" ? list.OrderBy(e => e.DiscountAmount) : list.OrderByDescending(e => e.DiscountAmount);
                    break;
                case "minAmount":
                    list = sortOrder == "asc" ? list.OrderBy(e => e.MinAmount) : list.OrderByDescending(e => e.MinAmount);
                    break;
                case "maxAmount":
                    list = sortOrder == "asc" ? list.OrderBy(e => e.MaxAmount) : list.OrderByDescending(e => e.MaxAmount);
                    break;
                case "status":
                    list = sortOrder == "asc" ? list.OrderBy(e => e.Status) : list.OrderByDescending(e => e.Status);
                    break;
                case "createdBy":
                    list = sortOrder == "asc" ? list.OrderBy(e => e.CreatedBy) : list.OrderByDescending(e => e.CreatedBy);
                    break;
                case "createdDate":
                    list = sortOrder == "asc" ? list.OrderBy(e => e.CreatedDate) : list.OrderByDescending(e => e.CreatedDate);
                    break;
                default:
                    list = list.OrderByDescending(e => e.Id);
                    break;
            }
            return list;
        }

        public async Task<Models.Coupon?> GetCouponById(int id)
        {
            return await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<BaseResponse<MemoryStream>> ImportCoupons(IFormFile excelFile, string userId)
        {
            var coupons = new List<Models.Coupon>();
            var errorDetails = new List<(int RowIndex, List<string> Errors)>();

            try
            {
                var contentPath = environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads\\Templates");
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(path, excelFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await excelFile.CopyToAsync(stream);
                }

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        bool isHeaderSkipped = false;
                        int rowIndex = 0;

                        while (reader.Read())
                        {
                            rowIndex++;
                            var validationErrors = new List<string>();

                            if (!isHeaderSkipped)
                            {
                                isHeaderSkipped = true; // Skip header row
                                continue;
                            }

                            // Validation
                            var couponCode = reader.GetValue(0)?.ToString();
                            if (string.IsNullOrWhiteSpace(couponCode))
                            {
                                validationErrors.Add("Coupon Code is required.");
                            }

                            // Check for duplicate CouponCode in the database
                            var existingCoupon = await _dbContext.Coupons
                                .FirstOrDefaultAsync(c => c.CouponCode == couponCode);

                            if (existingCoupon != null)
                            {
                                validationErrors.Add($"Coupon Code '{couponCode}' already exists.");
                            }

                            if (!int.TryParse(reader.GetValue(1)?.ToString(), out int quantity) || quantity < 0)
                            {
                                validationErrors.Add("Quantity must be a non-negative integer.");
                            }

                            if (!double.TryParse(reader.GetValue(2)?.ToString(), out double discountAmount) || discountAmount < 0)
                            {
                                validationErrors.Add("Discount Amount must be a non-negative number.");
                            }

                            // Validation for Min Amount
                            double? minAmount = null;
                            if (!string.IsNullOrWhiteSpace(reader.GetValue(3)?.ToString()))
                            {
                                if (double.TryParse(reader.GetValue(3)?.ToString(), out double min))
                                {
                                    minAmount = min;
                                }
                                else
                                {
                                    validationErrors.Add("Min Amount must be a valid number.");
                                }
                            }

                            // Validation for Max Amount
                            double? maxAmount = null;
                            if (!string.IsNullOrWhiteSpace(reader.GetValue(4)?.ToString()))
                            {
                                if (double.TryParse(reader.GetValue(4)?.ToString(), out double max))
                                {
                                    maxAmount = max;
                                }
                                else
                                {
                                    validationErrors.Add("Max Amount must be a valid number.");
                                }
                            }

                            // Validation Status
                            if (!bool.TryParse(reader.GetValue(5)?.ToString(), out bool status))
                            {
                                validationErrors.Add("Status must be 'true' or 'false'.");
                            }

                            // Skip row if there are validation errors
                            if (validationErrors.Count > 0)
                            {
                                errorDetails.Add((rowIndex, validationErrors));
                                continue;
                            }

                            var coupon = new Models.Coupon
                            {
                                CouponCode = couponCode,
                                Quantity = quantity,
                                DiscountAmount = discountAmount,
                                MinAmount = minAmount,
                                MaxAmount = maxAmount,
                                Status = status,
                                CreatedBy = userId,
                                CreatedDate = DateTime.UtcNow,
                            };

                            coupons.Add(coupon);
                        }
                    }
                }

                // Handle errors
                if (errorDetails.Any())
                {
                    return CreateErrorReport(errorDetails); // Create error report here
                }

                // Save valid coupons to the database
                if (coupons.Any())
                {
                    await _dbContext.Coupons.AddRangeAsync(coupons);
                    await _dbContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions (logging, etc.)
                throw new BadRequestException("Error occurred during file import: " + ex.Message);
            }

            return new BaseResponse<MemoryStream>(null); // Return a success response
        }


        private BaseResponse<MemoryStream> CreateErrorReport(List<(int RowIndex, List<string> Errors)> errorDetails)
        {
            using (var errorWorkbook = new XLWorkbook())
            {
                var worksheet = errorWorkbook.Worksheets.Add("Errors Report");

                // Add header
                worksheet.Cell(1, 1).Value = "Row Index";
                worksheet.Cell(1, 2).Value = "Error Messages";

                int errorRowIndex = 2;

                foreach (var (rowIndex, errors) in errorDetails)
                {
                    foreach (var error in errors)
                    {
                        worksheet.Cell(errorRowIndex, 1).Value = rowIndex;
                        worksheet.Cell(errorRowIndex, 2).Value = error;
                        errorRowIndex++;
                    }
                }

                using (var errorMemoryStream = new MemoryStream())
                {
                    errorWorkbook.SaveAs(errorMemoryStream);
                    errorMemoryStream.Position = 0; // Reset stream position for reading
                    return new BaseResponse<MemoryStream>(errorMemoryStream); // Return the MemoryStream
                }
            }
        }


    }

}
