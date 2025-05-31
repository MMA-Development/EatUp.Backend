using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Users
{
    public class UserUnFavoritedAMealEvent: IEvent
    {
        public UserUnFavoritedAMealEvent(Guid id)
        {
            Id = id;
        }
        public UserUnFavoritedAMealEvent()
        {
            
        }
        public Guid Id { get; set; }
    }
}
