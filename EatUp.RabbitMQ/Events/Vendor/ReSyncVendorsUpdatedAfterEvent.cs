using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Vendor
{
    public class ReSyncVendorsUpdatedAfterEvent(DateTime updatedAfter, string routingKey): IEvent
    {
        public DateTime UpdatedAfter { get; set; } = updatedAfter;
        public string RoutingKey { get; set; } = routingKey;
    }
}
