using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;
using FluentValidation;

namespace Catalog.API.Products.CreateProduct
{

    public record CreateProductCommand(ProductCreateDTO ProductCreateDTO) 
        : ICommand<CreateProductResult>;
    public record CreateProductResult(BaseResponse<string> Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.ProductCreateDTO.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.ProductCreateDTO.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
            RuleFor(x => x.ProductCreateDTO.Description).NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.ProductCreateDTO.Image).NotEmpty().WithMessage("Image is required");
            RuleFor(x => x.ProductCreateDTO.Color).NotEmpty().WithMessage("Color is required");
            RuleFor(x => x.ProductCreateDTO.Quantity).NotEmpty().WithMessage("Quantity is required");
            RuleFor(x => x.ProductCreateDTO.Brand).NotEmpty().WithMessage("Brand is required");
            RuleFor(x => x.ProductCreateDTO.Device).NotEmpty().WithMessage("Device is required");




        }
    }


    internal class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
    {

        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUploadImageRepository _uploadImageRepository;

        public CreateProductCommandHandler(IProductCategoryRepository productCategoryRepository, 
            IProductRepository productRepository,
            IHttpContextAccessor httpContextAccessor,
            IUploadImageRepository uploadImageRepository)
        {
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
            _uploadImageRepository = uploadImageRepository;
        }
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            //create Product entity from command object
            //save to database
            //return CreateProductResult result
            var uuid = Guid.NewGuid();
            var product = new Product
            {
                Name = command.ProductCreateDTO.Name,
                Price = command.ProductCreateDTO.Price,
                ImageUrl = uuid.ToString()+".jpg",
                Description = command.ProductCreateDTO.Description,
                Status = command.ProductCreateDTO.Status
            };
            var user = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(user)) throw new BadRequestException("User Id Is Null");

            await _productRepository.Create(product, user, cancellationToken);
            _productCategoryRepository.CreateProductCategories(
                command.ProductCreateDTO.Brand, command.ProductCreateDTO.Device
                , command.ProductCreateDTO.Color
                , product.ProductId, command.ProductCreateDTO.Quantity
                , command.ProductCreateDTO.Status, user);
            _uploadImageRepository.UploadFile(command.ProductCreateDTO.Image
                , product.ImageUrl);

            var response = new BaseResponse<string>(product.ProductId);

            //return result
            return new CreateProductResult(response);

        }
    }
}

