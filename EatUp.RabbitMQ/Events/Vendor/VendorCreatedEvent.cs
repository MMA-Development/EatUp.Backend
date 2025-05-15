using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Vendor
{
    public class VendorCreatedEvent: IEvent
    {
        public string Name { get; set; }
        public string Cvr { get; set; }
        public string StripeAccountId { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
