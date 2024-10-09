namespace Catalog.API.Models.DTO
{
    public class UpdateQuantityForOrder
    {
        public string color { get; set; }
        public string productId { get; set; }
        public bool status { get; set; }
        public int quantity { get; set; }
        public string user { get; set; }
        public bool IsCancel { get; set; }
    }
}
