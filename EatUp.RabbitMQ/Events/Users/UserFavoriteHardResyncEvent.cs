using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Users
{
    public class UserFavoriteHardResyncEvent: IEvent
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MealId { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
