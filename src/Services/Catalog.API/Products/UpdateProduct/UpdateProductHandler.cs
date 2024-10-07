using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Models;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;
using FluentValidation;

namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(ProductUpdateDTO ProductUpdateDTO)
        : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(command => command.ProductUpdateDTO.Id).NotEmpty().WithMessage("Product ID is required");

            RuleFor(command => command.ProductUpdateDTO.Name)
                .NotEmpty().WithMessage("Name is required");

            RuleFor(command => command.ProductUpdateDTO.Price).GreaterThan(0).WithMessage("Price must be greater than 0");

        }
    }

    internal class UpdateProductCommandHandler
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUploadImageRepository _uploadImageRepository;

        public UpdateProductCommandHandler(IProductCategoryRepository productCategoryRepository,
            IProductRepository productRepository,
            IHttpContextAccessor httpContextAccessor,
            IUploadImageRepository uploadImageRepository)
        {
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
            _uploadImageRepository = uploadImageRepository;
        }
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {

            var product = await _productRepository.GetProductByID(command.ProductUpdateDTO.Id);

            if (product is null)
            {
                throw new ProductNotFoundException(command.ProductUpdateDTO.Id);
            }
            var user = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();

          
            product.Name = command.ProductUpdateDTO.Name;
            product.Price = command.ProductUpdateDTO.Price;
            product.Description = command.ProductUpdateDTO.Description;
            product.Status = command.ProductUpdateDTO.Status;

            await _productRepository.Update(product,user,cancellationToken);
            if (command.ProductUpdateDTO.Image != null)
            {
                _uploadImageRepository.UploadFile(command.ProductUpdateDTO.Image, product.ImageUrl);
            }
            _productCategoryRepository.UpdateBrand(command.ProductUpdateDTO.Brand,command.ProductUpdateDTO.Id,command.ProductUpdateDTO.Status,user);
            _productCategoryRepository.UpdateDevice(command.ProductUpdateDTO.Device,command.ProductUpdateDTO.Id,command.ProductUpdateDTO.Status,user);
            _productCategoryRepository.UpdateColor(command.ProductUpdateDTO.Color,command.ProductUpdateDTO.Id,command.ProductUpdateDTO.Status,command.ProductUpdateDTO.Quantity, user);



            return new UpdateProductResult(true);
        }
    }
}
