using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Orders.EventHandlers
{
    public class VendorCreatedEventHandler(IBaseRepository<VendorProjection> repository) : IEventHandler<VendorCreatedEvent>
    {
        public async Task HandleAsync(VendorCreatedEvent @event)
        {
            var existing = await repository.GetById(@event.Id);
            if (existing != null)
            {
                Console.WriteLine($"Vendor already exists {@event.Id}");
                return;
            }

            var vendor = VendorProjection.FromVendorCreatedEvent(@event);
            await repository.Insert(vendor);
            await repository.Save();
        }
    }
}
