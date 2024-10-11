using BuildingBlocks.Messaging.Events.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Events
{
    public record ReduceQuantityEvent : IntegrationEvent
    {
        public List<ReduceQuantityDTO> listProductCategory { get; set; }
    }
}
