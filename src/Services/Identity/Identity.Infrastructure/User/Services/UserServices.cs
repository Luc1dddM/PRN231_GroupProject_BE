using BuildingBlocks.Models;
using Identity.Application.File.Services;
using Identity.Application.User.Dtos;
using Identity.Application.User.Interfaces;
using Identity.Application.Utils;
using Identity.Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.User.Services
{
    public class UserServices : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileSerivce _fileService;
        private readonly UserManager<Domain.Entities.User> _userManager;
        public UserServices(ApplicationDbContext context, UserManager<Domain.Entities.User> userManager, IFileSerivce fileSerivce)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileSerivce;
        }

        public async Task<BaseResponse<Domain.Entities.User>> CreateNewUser(CreateNewUserDto dto)
        {
            try
            {
                var user = dto.Adapt<Domain.Entities.User>();

                if (dto.ImageFile is not null)
                {
                    var fileResult = _fileService.SaveImage(dto.ImageFile);
                    if (fileResult.Item1 == 1)
                    {
                        user.ProfilePicture = fileResult.Item2;
                    }
                }
                user.Id = Guid.NewGuid().ToString();
                user.EmailConfirmed = true;
                user.CreatedAt = DateTime.Now;
                user.CreatedBy = dto.CreatedBy;
                user.UserName = dto.Email;

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, dto.Role);
                    return new BaseResponse<Domain.Entities.User>(user);
                }
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BaseResponse<Domain.Entities.User>(null, errorMessages);
            }
            catch (Exception ex)
            {
                return new BaseResponse<Domain.Entities.User>(null, new List<string>() { ex.Message });
            }
        }

        public async Task<BaseResponse<PaginatedList<Domain.Entities.User>>> GetAllUser(GetListUserParamsDto parameters)
        {
            try
            {
                IQueryable<Domain.Entities.User> query = _context.Users.AsQueryable();

                // Step 1: Apply filters (e.g., Status and Dob)
                if (parameters.Statuses is not null && parameters.Dob is not null)
                {
                    query = Filter(parameters.Statuses, parameters.Dob, query);
                }

                // Step 2: Apply keyword search
                query = Search(query, parameters?.Keyword ?? "");

                // Step 3: Apply sorting
                query = SortUser(parameters.SortBy, parameters.SortOrder, query);

                // Step 4: Fetch all users who meet the criteria
                var allUsers = await query.AsNoTracking().ToListAsync();

                // Step 5: Filter users who are not "Customer" or "Admin" (client-side filtering)
                var employeeList = new List<Domain.Entities.User>();
                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (!roles.Contains("Admin"))
                    {
                        employeeList.Add(user);
                    }
                }

                // Step 6: Apply pagination on the filtered employee list
                var employees = await PaginatedList<Domain.Entities.User>.CreateAsync(employeeList.AsQueryable(), parameters.PageNumber, parameters.PageSize);

                return new BaseResponse<PaginatedList<Domain.Entities.User>>(employees);
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginatedList<Domain.Entities.User>>(null, new List<string>() { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> UpdateUser(string id, UpdateUserDto request)
        {
            try
            {
                if (id != request.Id)
                {
                    return new BaseResponse<bool>(null, new List<string>() { "Id in Url and request not match" });
                }

                var User = await _userManager.FindByIdAsync(id);
                if (User is null)
                {
                    return new BaseResponse<bool>(null, new List<string>() { $"User With Id: {id} does not exist! " });
                }

                if (request.ImageFile != null)
                {
                    var fileResult = _fileService.SaveImage(request.ImageFile);
                    if (fileResult.Item1 == 1)
                    {
                        User.ProfilePicture = fileResult.Item2;
                    }

                    var oldRole = await _userManager.GetRolesAsync(User);
                    var result = await _userManager.RemoveFromRolesAsync(User, oldRole);
                    string errorMessages = "";
                    if (!result.Succeeded)
                    {
                        errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception(errorMessages);
                    }

                    result = await _userManager.AddToRoleAsync(User, request.Role);

                    if (!result.Succeeded)
                    {
                        errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception(errorMessages);
                    }

                    string oldImage = User.ProfilePicture ?? "";

                    User.Email = request.Email;
                    User.PasswordHash = request.Password;
                    User.PhoneNumber = request.PhoneNumber;
                    User.FullName = request.FullName;
                    User.IsActive = request.IsActive;
                    User.BirthDay = request.BirthDay;
                    User.UpdatedBy = request.UpdatedBy;
                    User.UpdatedAt = DateTime.Now;

                    result = await _userManager.UpdateAsync(User);

                    if (!result.Succeeded)
                    {
                        errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception(errorMessages);
                    }

                    if (request.ImageFile != null)
                    {
                        await _fileService.DeleteImage(oldImage);
                    }
                    return new BaseResponse<bool>(true);
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>(null, new List<string>() { ex.Message });
            }

        }

        private IQueryable<Domain.Entities.User> Filter(string[] statuses, DateOnly? dob, IQueryable<Domain.Entities.User> list)
        {
            if (dob is not null)
            {
                list = list.Where(e => e.BirthDay == dob);
            }
            if (statuses != null && statuses.Length > 0)
            {
                list = list.Where(e => statuses.Contains(e.IsActive.ToString()));
            }

            return list;
        }



        private IQueryable<Domain.Entities.User> Search(IQueryable<Domain.Entities.User> list, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                list = list.Where(u =>
                    u.FullName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.PhoneNumber.Contains(searchTerm));
            }
            return list;
        }

        private IQueryable<Domain.Entities.User> SortUser(string sortBy, string sortOrder, IQueryable<Domain.Entities.User> list)
        {
            switch (sortBy)
            {
                case "name":
                    list = sortOrder == "asc" ? list.OrderBy(u => u.FullName) : list.OrderByDescending(u => u.FullName);
                    break;
                case "email":
                    list = sortOrder == "asc" ? list.OrderBy(u => u.Email) : list.OrderByDescending(u => u.Email);
                    break;
                case "phonenumber":
                    list = sortOrder == "asc" ? list.OrderBy(u => u.PhoneNumber) : list.OrderByDescending(u => u.PhoneNumber);
                    break;
                case "status":
                    list = sortOrder == "asc" ? list.OrderBy(u => u.IsActive) : list.OrderByDescending(u => u.IsActive);
                    break;
                case "birthday":
                    list = sortOrder == "asc" ? list.OrderBy(u => u.BirthDay) : list.OrderByDescending(u => u.BirthDay);
                    break;
                default:
                    list = list.OrderByDescending(u => u.CreatedAt);
                    break;
            }
            return list;
        }
    }
}
