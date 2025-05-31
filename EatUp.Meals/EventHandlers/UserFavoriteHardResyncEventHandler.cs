using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Meals.EventHandlers
{
    public class UserFavoriteHardResyncEventHandler(IRepository<UserFavoriteMealsProjection> repository) : IEventHandler<UserFavoriteHardResyncEvent>
    {
        public async Task HandleAsync(UserFavoriteHardResyncEvent @event)
        {
            var existing = await repository.GetById(@event.Id, true);
            if (existing == null)
            {
                existing = UserFavoriteMealsProjection.FromEvent(@event);
                await repository.Insert(existing);
            }
            else
            {
                UserFavoriteMealsProjection.MergeEvent(existing, @event);
            }
            await repository.Save();
        }
    }
}
