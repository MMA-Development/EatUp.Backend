using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Meals;

namespace EatUp.Orders.EventHandlers
{
    public class MealUpdatedEventHandler(IBaseRepository<MealProjection> repository) : IEventHandler<MealUpdatedEvent>
    {
        public async Task HandleAsync(MealUpdatedEvent @event)
        {
            var existing = await repository.GetById(@event.Id, tracking: true);
            if (existing == null) 
            {
                Console.WriteLine($"Meal with ID {@event.Id} not found for update.");
                return;
            }

            MealProjection.Update(existing, @event);
            await repository.Save();
        }
    }
}
