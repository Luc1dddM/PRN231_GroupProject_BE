using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Events
{
    public class ReduceQuantityEvent
    {
        public string color { get; set; }
        public string productId { get; set; }
        public bool status { get; set; }
        public int quantity { get; set; }
        public string user { get; set; }
        public bool IsCancel {  get; set; }
    }
}
