using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Order;

namespace EatUp.Meals.EventHandlers
{
    public class OrderCompletedEventHandler(IRepository<CompletedOrderProjection> repository) : IEventHandler<OrderCompletedEvent>
    {
        public async Task HandleAsync(OrderCompletedEvent @event)
        {
            var compltedOrderProjection = CompletedOrderProjection.FromEvent(@event);
            await repository.Insert(compltedOrderProjection);
        }
    }
}
