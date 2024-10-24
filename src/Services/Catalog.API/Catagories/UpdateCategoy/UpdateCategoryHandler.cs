using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Catalog.API.Exceptions;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;
using FluentValidation;

namespace Catalog.API.Categories.UpdateCategory
{
    public record UpdateCategoryCommand(string Id, string Name, string Type, bool Status)
        : ICommand<UpdateCategoryResult>;
    public record UpdateCategoryResult(BaseResponse<bool> IsSuccess);

    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty().WithMessage("Product ID is required");

            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required");

            RuleFor(command => command.Type).NotEmpty().WithMessage("Type is required");


        }
    }

    internal class UpdateCategoryCommandHandler
        : ICommandHandler<UpdateCategoryCommand, UpdateCategoryResult>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<UpdateCategoryResult> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {

            var category = _categoryRepository.GetCategoryByID(command.Id);

            if (category is null)
            {
                throw new ProductNotFoundException(command.Id);
            }
            var user = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();

          
            category.Name = command.Name;
            category.Type = command.Type;
            category.Status = command.Status;


            _categoryRepository.update(category, user);


            return new UpdateCategoryResult(new BaseResponse<bool>(true,"Update category successfuly"));
        }
    }
}
