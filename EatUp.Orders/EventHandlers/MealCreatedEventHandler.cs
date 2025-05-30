using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Meals;

namespace EatUp.Orders.EventHandlers
{
    public class MealCreatedEventHandler(IBaseRepository<MealProjection> repository) : IEventHandler<MealCreatedEvent>
    {
        public async Task HandleAsync(MealCreatedEvent @event)
        {
            var existing = await repository.GetById(@event.Id);
            if (existing != null)
            {
                Console.WriteLine($"Meal already exists, skipping insertion {@event.Id}");
                return;
            }
            var meal = MealProjection.FromCreatedEvent(@event);
            await repository.Insert(meal);
            await repository.Save();
        }
    }
}
