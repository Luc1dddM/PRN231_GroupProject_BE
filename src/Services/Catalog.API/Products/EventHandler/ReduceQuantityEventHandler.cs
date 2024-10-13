using BuildingBlocks.Messaging.Events;
using Catalog.API.Models.DTO;
using Catalog.API.Repository;
using MassTransit;

namespace Catalog.API.Products.EventHandler
{
    public class ReduceQuantityEventHandler : IConsumer<ReduceQuantityEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IProductCategoryRepository _productCategoruRepository;



        public ReduceQuantityEventHandler(
            IPublishEndpoint publishEndpoint,
            IProductCategoryRepository productCategoryRepository)
        {
            _publishEndpoint = publishEndpoint;
            _productCategoruRepository = productCategoryRepository;
           
        }

        public Task Consume(ConsumeContext<ReduceQuantityEvent> context)
        {
            try
            {
                var list = context.Message.Adapt<UpdateQuantityForOrder>();
                foreach (var item in list.listProductCatgory)
                {
                    _productCategoruRepository.UpdateQuantityForOrder(item.productCategoryId,item.quantity,item.user,item.IsCancel);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
