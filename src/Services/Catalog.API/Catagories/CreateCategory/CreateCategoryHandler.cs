using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;
using FluentValidation;

namespace Catalog.API.Categories.CreateCategory
{

    public record CreateCategoryCommand(string Name, string Type, bool Status) 
        : ICommand<CreateCategoryResult>;
    public record CreateCategoryResult(BaseResponse<string> Id);

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Type).NotEmpty().WithMessage("Type is required");
            RuleFor(x => x.Status).NotNull().WithMessage("Status is required");
        }
    }


    internal class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, CreateCategoryResult>
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CreateCategoryResult> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            //create Product entity from command object
            //save to database
            //return CreateProductResult result
            var category = new Category
            {
                Name = command.Name,
                Type = command.Type,
                Status = command.Status
                
            };
            var user = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(user)) throw new BadRequestException("User Id Is Null");


            _categoryRepository.Create(category,user);

            var result = new BaseResponse<string>(category.CategoryId, "Create category successfully");

            //return result
            return new CreateCategoryResult(result);

        }
    }
}

