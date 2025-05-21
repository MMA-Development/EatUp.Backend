using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Orders.EventHandlers
{
    public class UserDeletedEventHandler(IBaseRepository<UserProjection> repository) : IEventHandler<UserDeletedEvent>
    {
        public async Task HandleAsync(UserDeletedEvent @event)
        {
            await repository.Delete(@event.Id);
        }
    }
}
