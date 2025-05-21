using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Orders.EventHandlers
{
    public class UserCreatedEventHandler(IBaseRepository<UserProjection> repository) : IEventHandler<UserCreatedEvent>
    {
        public async Task HandleAsync(UserCreatedEvent @event)
        {
            var existing = await repository.GetById(@event.Id);
            if (existing != null)
            {
                Console.WriteLine($"User alread exist with id: {@event.Id}, name: {@event.Fullname}");
            }

            UserProjection user = UserProjection.FromCreatedEvent(@event);
            await repository.Insert(user);
            await repository.Save();
        }
    }
}
