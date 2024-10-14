﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Events
{
    public class CouponQuantityUsedEvent
    {
        public string CouponCode { get; set; }
        public int QuantityUsed { get; set; }
    }
}
