using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Meals.EventHandlers
{
    public class UserFavoritedAMealEventHandler(IRepository<UserFavoriteMealsProjection> repository) : IEventHandler<UserFavoritedAMealEvent>
    {
        public async Task HandleAsync(UserFavoritedAMealEvent @event)
        {
            var favorite = UserFavoriteMealsProjection.FromEvent(@event);
            await repository.Insert(favorite);
            await repository.Save();
        }
    }
}
