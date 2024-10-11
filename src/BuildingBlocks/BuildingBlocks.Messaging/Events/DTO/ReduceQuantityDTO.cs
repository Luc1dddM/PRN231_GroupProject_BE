using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Events.DTO
{
    public class ReduceQuantityDTO
    {
        public string productCategoryId { get; set; }
        public int quantity { get; set; }
        public string user { get; set; }
        public bool IsCancel { get; set; }
    }
}
