using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Meals.EventHandlers
{
    public class UserUnFavoritedAMealEventHandler(IRepository<UserFavoriteMealsProjection> repository) : IEventHandler<UserUnFavoritedAMealEvent>
    {
        public async Task HandleAsync(UserUnFavoritedAMealEvent @event)
        {
            await repository.Delete(@event.Id);
            await repository.Save();
        }
    }
}
