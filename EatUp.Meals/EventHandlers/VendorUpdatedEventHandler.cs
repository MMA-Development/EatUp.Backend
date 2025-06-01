using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Meals.EventHandlers
{
    public class VendorUpdatedEventHandler(IRepository<VendorProjection> repository) : IEventHandler<VendorUpdatedEvent>
    {
        public async Task HandleAsync(VendorUpdatedEvent @event)
        {
            var vendor = await repository.GetById(@event.Id, true, ignoreFilters: true);
            VendorProjection.Merge(vendor, @event);
            await repository.Save();
        }
    }
}
