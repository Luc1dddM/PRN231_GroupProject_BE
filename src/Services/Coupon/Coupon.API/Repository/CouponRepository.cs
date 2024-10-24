

using BuildingBlocks.Models;
using Coupon.API.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Coupon.API.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly Prn231GroupProjectContext _dbContext;

        public CouponRepository(Prn231GroupProjectContext dbContext)
        {
            _dbContext = dbContext;
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
                query = Filter(parameters.Statuse, parameters.MinAmount, parameters.MaxAmount, query);
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

        private IQueryable<Models.Coupon> Filter(string[] statuses, double? minAmount, double? maxAmount, IQueryable<Models.Coupon> list)
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


    }
}
