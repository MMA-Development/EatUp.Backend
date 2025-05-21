using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Meals;

namespace EatUp.Orders.EventHandlers
{
    public class MealDeletedEventHandler(IBaseRepository<MealProjection> repository) : IEventHandler<MealDeletedEvent>
    {
        public async Task HandleAsync(MealDeletedEvent @event)
        {
            await repository.Delete(@event.Id);
            await repository.Save();
        }
    }
}
