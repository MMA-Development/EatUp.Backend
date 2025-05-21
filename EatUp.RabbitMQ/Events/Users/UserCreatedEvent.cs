using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Users
{
    public class UserCreatedEvent: IEvent
    {
        public Guid Id { get; set; }

        public string Fullname { get; set; }
        public string Email { get; set; }
    }
}
