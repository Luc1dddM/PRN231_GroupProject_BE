using BuildingBlocks.CQRS;
using Catalog.API.Models.DTO;
using Catalog.API.Models;
using Catalog.API.Repository;
using FluentValidation;
using BuildingBlocks.Models;
using BuildingBlocks.Exceptions;

namespace Catalog.API.Products.AddColor
{
    public record AddColorCommand(string ColorId, string ProductId, int Quantity, bool Status)
        : ICommand<AddColorResult>;
    public record AddColorResult(BaseResponse<string> Id);

    public class AddColorCommandValidator : AbstractValidator<AddColorCommand>
    {
        public AddColorCommandValidator()
        {
            RuleFor(x => x.ColorId).NotEmpty().WithMessage("Color is required");
            RuleFor(x => x.Quantity).GreaterThan(-1).WithMessage("Quantity must be positive");
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
        }
    }


    internal class AddColorCommandHandler : ICommandHandler<AddColorCommand, AddColorResult>
    {


        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AddColorCommandHandler(
            IProductCategoryRepository productCategoryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _productCategoryRepository = productCategoryRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<AddColorResult> Handle(AddColorCommand command, CancellationToken cancellationToken)
        {
            //create Product entity from command object
            //save to database
            //return CreateProductResult result
            var productCategory = new ProductCategory()
            {
                ProductId = command.ProductId,
                Quantity = command.Quantity,
                CategoryId = command.ColorId,
                Status = command.Status
                
            };
            var user = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(user)) throw new BadRequestException("User Id Is Null");
            _productCategoryRepository.AddColor(productCategory, user);

            var result = new BaseResponse<string>(productCategory.ProductCategoryId, "Add color successfully");

            //return result
            return new AddColorResult(result);

        }
    }

}
