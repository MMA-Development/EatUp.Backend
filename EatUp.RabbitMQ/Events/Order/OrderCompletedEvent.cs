using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Order
{
    public class OrderCompletedEvent:  IEvent
    {
        public Guid Id { get; set; }

        public Guid VendorId { get; set; }

        public Guid UserId { get; set; }

        public Guid MealId { get; set; }

        public int Quantity { get; set; }
        public double Price { get; set; }
        public double OriginalPrice { get; set; }
    }
}
