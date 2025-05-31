using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Users
{
    public class UserFavoritedAMealEvent(Guid id, Guid userId, Guid mealId): IEvent
    {
        public Guid Id { get; set; } = id;
        public Guid UserId { get; set; } = userId;
        public Guid MealId { get; set; } = mealId;
    }
}
