using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Meals
{
    public class ReSyncMealsUpdatedAfterEvent(DateTime after, string queue): IEvent
    {
        public DateTime After { get; set; } = after;
        public string Queue { get; set; } = queue;
    }
}
