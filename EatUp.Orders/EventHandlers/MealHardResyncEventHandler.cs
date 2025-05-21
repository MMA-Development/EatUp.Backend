using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Meals;

namespace EatUp.Orders.EventHandlers
{
    public class MealHardResyncEventHandler(IBaseRepository<MealProjection> repository) : IEventHandler<MealHardResyncEvent>
    {
        public async Task HandleAsync(MealHardResyncEvent @event)
        {
            var existing = await repository.GetById(@event.Id, tracking: true, ignoreFilters: true);
            if (existing == null)
            {
                var meal = MealProjection.FromHardResyncEvent(@event);
                await repository.Insert(meal);
            }
            else
            {
                MealProjection.HardResync(existing, @event);
            }

            await repository.Save();
        }
    }
}
