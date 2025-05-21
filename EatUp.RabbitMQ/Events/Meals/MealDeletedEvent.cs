using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Meals
{
    public class MealDeletedEvent(Guid id): IEvent
    {
        public Guid Id { get; set; } = id;
    }
}
