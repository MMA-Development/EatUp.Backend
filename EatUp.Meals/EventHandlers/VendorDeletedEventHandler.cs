using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Meals.EventHandlers
{
    public class VendorDeletedEventHandler(IRepository<VendorProjection> repository) : IEventHandler<VendorDeletedEvent>
    {
        public async Task HandleAsync(VendorDeletedEvent @event)
        {
            await repository.Delete(@event.Id);
            await repository.Save();
        }
    }
}
