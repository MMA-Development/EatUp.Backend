using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;
using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Orders.EventHandlers
{
    public class UserHardResyncEventHandler(IBaseRepository<UserProjection> repository) : IEventHandler<UserHardResyncEvent>
    {
        public async Task HandleAsync(UserHardResyncEvent @event)
        {
            var projection = await repository.GetById(@event.Id, tracking: true, ignoreFilters: true);
            if (projection == null)
            {
                var user = UserProjection.FromHardResyncEvent(@event);
                await repository.Insert(user);
            }
            else
            {
                UserProjection.HardResync(projection, @event);
            }
            await repository.Save();
        }
    }
}
