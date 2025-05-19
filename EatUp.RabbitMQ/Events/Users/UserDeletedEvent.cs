using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Users
{
    public class UserDeletedEvent(Guid id): IEvent
    {
        public Guid Id { get; set; } = id;
    }
}
