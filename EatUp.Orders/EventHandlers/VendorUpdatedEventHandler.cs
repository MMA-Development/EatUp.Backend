using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Orders.EventHandlers
{
    public class VendorUpdatedEventHandler(IBaseRepository<VendorProjection> repository) : IEventHandler<VendorUpdatedEvent>
    {
        public async Task HandleAsync(VendorUpdatedEvent @event)
        {
            var existing = await repository.GetById(@event.Id, true);
            if (existing == null)
            {
                Console.WriteLine($"Vendor with ID {@event.Id} not found.");
            }
            VendorProjection.Merge(existing, @event);
            await repository.Save();
        }
    }
}
