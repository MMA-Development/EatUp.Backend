using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Orders.EventHandlers
{
    public class UserUpdatedEventHandler(IBaseRepository<UserProjection> repository) : IEventHandler<UserUpdatedEvent>
    {
        public async Task HandleAsync(UserUpdatedEvent @event)
        {
            var existing = await repository.GetById(@event.Id, true);
            if (existing == null)
            {
                Console.WriteLine($"User doesnt exist with id: {@event.Id}, name: {@event.Fullname}");
                return;
            }
            UserProjection.MergeUpdatedEvent(existing, @event);
            await repository.Save();
        }
    }
}
