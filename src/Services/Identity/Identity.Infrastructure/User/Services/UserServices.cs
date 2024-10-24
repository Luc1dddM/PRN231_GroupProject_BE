using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using BuildingBlocks.Validation;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using Identity.Application.File.Services;
using Identity.Application.User.Dtos;
using Identity.Application.User.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

namespace Identity.Infrastructure.User.Services
{
    public class UserServices : IUserService
    {
        private IWebHostEnvironment environment;
        private readonly ApplicationDbContext _context;
        private readonly IFileSerivce _fileService;
        private readonly UserManager<Domain.Entities.User> _userManager;


        const string DefaultPassword = "123456";
        public UserServices(ApplicationDbContext context, UserManager<Domain.Entities.User> userManager, IFileSerivce fileSerivce, IWebHostEnvironment env)
        {
            environment = env;
            _context = context;
            _userManager = userManager;
            _fileService = fileSerivce;
        }

        public async Task<BaseResponse<UserDto>> CreateNewUser(CreateNewUserDto dto)
        {
            var user = dto.Adapt<Domain.Entities.User>();

            //Check if User is also Customer and Employee?
            if (dto.Roles.Contains("Customer") && dto.Roles.Count > 1)
                throw new BadRequestException("User cannot be also Customer and Employee");

            if (dto.ImageFile is not null)
            {
                var fileResult = _fileService.SaveImage(dto.ImageFile);
                if (fileResult.Item1 == 1)
                {
                    user.ProfilePicture = fileResult.Item2;
                }
            }
            user.UserId = Guid.NewGuid().ToString();
            user.EmailConfirmed = true;
            user.CreatedAt = DateTime.Now;
            user.CreatedBy = dto.CreatedBy;
            user.UserName = dto.Email;

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddToRolesAsync(user, dto.Roles);
                var userDto = user.Adapt<UserDto>();
                return new BaseResponse<UserDto>(userDto);
            }
            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception(errorMessages);
        }

        public async Task<BaseResponse<PaginatedList<UserDto>>> GetAllUser(GetListUserParamsDto parameters)
        {
            IQueryable<Domain.Entities.User> query = _context.Users.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).AsQueryable();

            // Step 1: Apply filters (e.g., Status and Dob)
            if (parameters.Statuses is not null || parameters.Genders is not null || parameters.Dob is not null)
            {
                query = Filter(parameters.Statuses, parameters.Genders, parameters.Dob, query);
            }

            // Step 2: Apply keyword search
            query = Search(query, parameters?.Keyword ?? "");

            // Step 3: Apply sorting
            query = SortUser(parameters.SortBy, parameters.SortOrder, query);

            // Step 4: Fetch all users who meet the criteria
            var allUsers = await query.AsNoTracking().ToListAsync();

            // Step 5: Filter users who are not "Customer" or "Admin" (client-side filtering)
            var listEmployeesDto = new List<UserDto>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    var userDto = user.Adapt<UserDto>();
                    userDto.Id = user.UserId;
                    userDto.Email = user.Email;
                    userDto.roles = roles;
                    userDto.UpdatedBy = user.UpdatedByUser?.FullName ?? string.Empty;
                    userDto.CreatedBy = user.CreatedByUser?.FullName ?? string.Empty;
                    listEmployeesDto.Add(userDto);
                }
            }


            // Step 6: Apply pagination on the filtered employee list
            var employees = await PaginatedList<UserDto>.CreateAsync(listEmployeesDto.AsQueryable(), parameters.PageNumber, parameters.PageSize);

            return new BaseResponse<PaginatedList<UserDto>>(employees);
        }

        public async Task<BaseResponse<UserDto>> GetUserById(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            var userDto = user.Adapt<UserDto>();
            if (!String.IsNullOrEmpty(user?.ProfilePicture) && user.ProfilePicture.StartsWith("http"))
            {
                userDto.ImageBase64 = user.ProfilePicture;
            }
            else
            {
                if (!String.IsNullOrEmpty(user?.ProfilePicture))
                {
                    userDto.ImageBase64 = "data:image/png;base64," + Convert.ToBase64String((File.ReadAllBytes(user.ProfilePicture)));
                }
            }

            if (user is not null)
            {
                return new BaseResponse<UserDto>(userDto);
            }
            throw new UserNotFoundException(id);
        }

        public async Task<BaseResponse<UserDto>> UpdateUser(string id, UpdateUserDto request)
        {

            if (id != request.Id)
                throw new BadRequestException("Id in Url and request not match");

            var User = await _userManager.Users.SingleOrDefaultAsync(u => u.UserId == id);
            if (User is null)
                throw new UserNotFoundException(id);

            //Check if User is also Customer and Employee?
            if(request.Roles != null)
            {
                if (request.Roles.Contains("Customer") && request.Roles.Count > 1)
                    throw new BadRequestException("User cannot be also Customer and Employee");


                var oldRole = await _userManager.GetRolesAsync(User);
                var result = await _userManager.RemoveFromRolesAsync(User, oldRole);
                await _userManager.AddToRolesAsync(User, request.Roles);
            }


            string oldImage;
            if (request.ImageFile != null)
            {
                oldImage = User.ProfilePicture ?? "";
                await _fileService.DeleteImage(oldImage);

                var fileResult = _fileService.SaveImage(request.ImageFile);
                if (fileResult.Item1 == 1)
                {
                    User.ProfilePicture = fileResult.Item2;
                }
            }

            User.Email = request.Email ?? User.Email;
            User.Gender = request.Gender ?? User.Gender;
            User.PhoneNumber = request.PhoneNumber ?? User.PhoneNumber;
            User.FullName = request.FullName ?? User.FullName;
            User.IsActive = request.IsActive ?? User.IsActive;
            User.BirthDay = request.BirthDay;
            User.UpdatedBy = request.UpdatedBy;
            User.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var resultDto = User.Adapt<UserDto>();
            resultDto.Id = User.UserId;
            resultDto.roles = request.Roles;
            return new BaseResponse<UserDto>(resultDto);
        }

        public async Task<BaseResponse<MemoryStream>> ImportUserTemplate(IFormFile excelFile, string userId)
        {
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

                List<Domain.Entities.User> templates = new List<Domain.Entities.User>();
                List<(int RowIndex, List<string> Errors)> errorDetails = new List<(int, List<string>)>();


                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        bool isHeaderSkipped = false;
                        int rowIndex = 0;

                        while (reader.Read())
                        {
                            rowIndex++;
                            List<string> validationErrors = new List<string>();

                            if (!isHeaderSkipped)
                            {
                                isHeaderSkipped = true; // Skip header row
                                continue;
                            }

                            /* === Validation  ===*/
                            var error = "";

                            //Validation Email
                            if (!ImportFieldValidation.IsValidateEmail(reader.GetValue(0)?.ToString() ?? "", out error))
                            {
                                validationErrors.Add(error);
                            }

                            //Validation Full Name
                            if(!ImportFieldValidation.IsValidString(reader.GetValue(1)?.ToString(), "Full Name", 6, null, out error))
                            {
                                validationErrors.Add(error);
                            }

                            //Validation Gender
                            if (!ImportFieldValidation.IsValidGender(reader.GetValue(2)?.ToString(), out error))
                            {
                                validationErrors.Add(error);
                            }

                            //Validation Status
                            if (!ImportFieldValidation.IsValidateBoolean(reader.GetValue(3)?.ToString(), out error))
                            {
                                validationErrors.Add(error);
                            }

                            //Skip row 
                            if(validationErrors.Count > 0)
                            {
                                errorDetails.Add((rowIndex, validationErrors));
                                continue;
                            }

                            var passwordHasher = new PasswordHasher<object>();
                            // Hash the password
                            var hashedPassword = passwordHasher.HashPassword(null, DefaultPassword);

                            var user = new Domain.Entities.User()
                            {
                                Email = reader.GetValue(0)?.ToString() ?? "",
                                NormalizedEmail = reader.GetValue(0)?.ToString().ToUpper() ?? "",
                                EmailConfirmed = true,
                                UserName = reader.GetValue(0)?.ToString() ?? "",
                                PasswordHash = hashedPassword,
                                FullName = reader.GetValue(1)?.ToString() ?? "",
                                Gender = reader.GetValue(2)?.ToString() ?? "",
                                IsActive = bool.Parse(reader.GetValue(3)?.ToString() ?? "False"),
                                BirthDay = !string.IsNullOrEmpty(reader.GetValue(4)?.ToString()) ? DateOnly.ParseExact(reader.GetValue(1)?.ToString(), "d/M/yyyy") : DateOnly.FromDateTime(DateTime.Now),
                                CreatedBy = "dc15ca24-a251-48a2-ba70-c570bccf9367",
                                CreatedAt = DateTime.Now
                            };
                            templates.Add(user);
                        }
                    }
                }

                if (errorDetails.Any())
                {
                    using (var errorWorkbook = new XLWorkbook())
                    {
                        var worksheet = errorWorkbook.Worksheets.Add("Errors Report");

                        // Add header
                        worksheet.Cell(1, 1).Value = "Row Index";
                        worksheet.Cell(1, 2).Value = "Error Messages";

                        int errorRowIndex = 2; // Start from the second row, assuming the first row is for headers

                        foreach (var (rowIndex, errors) in errorDetails)
                        {
                            // Split errors to handle multiple errors for the same row
                            foreach (var error in errors)
                            {
                                worksheet.Cell(errorRowIndex, 1).Value = rowIndex; // Row index from the original data
                                worksheet.Cell(errorRowIndex, 2).Value = error;    // Individual error message
                                errorRowIndex++; // Move to the next row for the next error
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

                // Save valid templates to the database
                if (templates.Any())
                {
                    await _context.Users.AddRangeAsync(templates);
                    await _context.SaveChangesAsync();
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                throw new BadRequestException("File Import cuar m bij loix.. hayx down fiel cuar t di tml");
            }
            
            return null;
        }

        private IQueryable<Domain.Entities.User> Filter(string[] statuses, string[] genders, DateOnly? dob, IQueryable<Domain.Entities.User> list)
        {
            if (dob is not null)
            {
                list = list.Where(e => e.BirthDay == dob);
            }
            if (statuses != null && statuses.Length > 0)
            {
                list = list.Where(e => statuses.Contains(e.IsActive.ToString()));
            }
            if (genders != null && genders.Length > 0)
            {
                list = list.Where(e => genders.Contains(e.Gender.ToString()));
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
                case "fullName":
                    list = sortOrder == "ascend" ? list.OrderBy(u => u.FullName) : list.OrderByDescending(u => u.FullName);
                    break;
                case "email":
                    list = sortOrder == "ascend" ? list.OrderBy(u => u.Email) : list.OrderByDescending(u => u.Email);
                    break;
                case "phoneNumber":
                    list = sortOrder == "ascend" ? list.OrderBy(u => u.PhoneNumber) : list.OrderByDescending(u => u.PhoneNumber);
                    break;
                case "status":
                    list = sortOrder == "ascend" ? list.OrderBy(u => u.IsActive) : list.OrderByDescending(u => u.IsActive);
                    break;
                case "birthday":
                    list = sortOrder == "ascend" ? list.OrderBy(u => u.BirthDay) : list.OrderByDescending(u => u.BirthDay);
                    break;
                default:
                    list = list.OrderByDescending(u => u.Id);
                    break;
            }
            return list;
        }
    }
}
