using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Order;
using EatUp.Users.Models;
using EatUp.Users.Repositories;

namespace EatUp.Users.EventHandlers
{
    public class OrderCompletedEventHandler(IRepository<OrderCompletedProjection> repository) : IEventHandler<OrderCompletedEvent>
    {
        public async Task HandleAsync(OrderCompletedEvent @event)
        {
            await repository.Insert(new OrderCompletedProjection
            {
                Id = @event.Id,
                UserId = @event.UserId,
                TotalPrice = @event.Price,
                OriginalPrice = @event.OriginalPrice,
                Quantity = @event.Quantity
            });
            await repository.Save();
        }
    }
}
