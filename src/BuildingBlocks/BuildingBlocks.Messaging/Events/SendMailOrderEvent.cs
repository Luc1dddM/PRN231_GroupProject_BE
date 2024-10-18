using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Events
{
    public class SendMailOrderEvent
    {
        public string OrderId { get; set; }
        public string UserEmail { get; set; }
        public string CouponCode { get; set; }
    }
}
