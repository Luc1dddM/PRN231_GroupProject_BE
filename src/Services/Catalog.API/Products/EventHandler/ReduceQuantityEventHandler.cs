using BuildingBlocks.Messaging.Events;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;
using MapsterMapper;
using MassTransit;

namespace Catalog.API.Products.EventHandler
{
    public class ReduceQuantityEventHandler : IConsumer<ReduceQuantityEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IProductCategoryRepository _productCategoruRepository;
        private readonly IMapper _mapper;


        public ReduceQuantityEventHandler(
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            IProductCategoryRepository productCategoryRepository)
        {
            _publishEndpoint = publishEndpoint;
            _productCategoruRepository = productCategoryRepository;
            _mapper = mapper;
        }

        public Task Consume(ConsumeContext<ReduceQuantityEvent> context)
        {


            var data = _mapper.Map<UpdateQuantityForOrder>(context.Message);

            _productCategoruRepository.UpdateQuantityForOrder(data.color, data.productId, data.quantity, data.user, data.IsCancel);

            return Task.CompletedTask;
        }
    }
}
