using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Events
{
    public record UpdateUserEvent:IntegrationEvent
    {
        public string UserId { get; set; } = default!;
        public bool IsChat { get; set; }
        public string Name { get; set; } = default!;
    }
}
