using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Meals
{
    public class MealCreatedEvent: IEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

    }
}
