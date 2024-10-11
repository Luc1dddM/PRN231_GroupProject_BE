using BuildingBlocks.Messaging.Events.DTO;

namespace Catalog.API.Models.DTO
{
    public class UpdateQuantityForOrder
    {
        public List<ReduceQuantityDTO> listProductCatgory {  get; set; }
    }
}
