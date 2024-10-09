using BuildingBlocks.Models;
using Identity.Application.File.Services;
using Identity.Application.User.Dtos;
using Identity.Application.User.Interfaces;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Identity;
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

        public async Task<BaseResponse<UserDto>> CreateNewUser(CreateNewUserDto dto)
        {
            var user = dto.Adapt<Domain.Entities.User>();

            //Check if User is also Customer and Employee?
            if (dto.Role.Contains("Customer") && dto.Role.Count > 1)
            {
                return new BaseResponse<UserDto>("User cannot be also Customer and Employee");
            };

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
                result = await _userManager.AddToRolesAsync(user, dto.Role);
                var userDto = user.Adapt<UserDto>();
                return new BaseResponse<UserDto>(userDto);
            }
            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
            return new BaseResponse<UserDto>(null, errorMessages);
        }

        public async Task<BaseResponse<PaginatedList<UserDto>>> GetAllUser(GetListUserParamsDto parameters)
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

            var listEmployeesDto = employeeList.Adapt<List<UserDto>>();

            // Step 6: Apply pagination on the filtered employee list
            var employees = await PaginatedList<UserDto>.CreateAsync(listEmployeesDto.AsQueryable(), parameters.PageNumber, parameters.PageSize);

            return new BaseResponse<PaginatedList<UserDto>>(employees);
        }

        public async Task<BaseResponse<UserDto>> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userDto = user.Adapt<UserDto>();
            if (user is not null)
            {
                return new BaseResponse<UserDto>(userDto);
            }
            throw new UserNotFoundException(id);
        }

        public async Task<BaseResponse<bool>> UpdateUser(string id, UpdateUserDto request)
        {

            if (id != request.Id)
            {
                return new BaseResponse<bool>("Id in Url and request not match");
            }

            var User = await _userManager.FindByIdAsync(id);
            if (User is null)
            {
                return new BaseResponse<bool>($"User With Id: {id} does not exist! ");
            }

            //Check if User is also Customer and Employee?
            if (request.Roles.Contains("Customer") && request.Roles.Count > 1)
            {
                return new BaseResponse<bool>("User cannot be also Customer and Employee");
            };

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

                result = await _userManager.AddToRolesAsync(User, request.Roles);

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
            return new BaseResponse<bool>(true);
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
