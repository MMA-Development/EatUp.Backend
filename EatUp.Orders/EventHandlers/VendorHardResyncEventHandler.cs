using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Orders.EventHandlers
{
    public class VendorHardResyncEventHandler(IBaseRepository<VendorProjection> repository) : IEventHandler<VendorHardResyncEvent>
    {
        public async Task HandleAsync(VendorHardResyncEvent @event)
        {
            var projection = await repository.GetById(@event.Id, tracking: true, ignoreFilters: true);
            if (projection == null)
            {
                var vendor = VendorProjection.FromVendorHardResyncEvent(@event);
                await repository.Insert(vendor);
            }
            else
            {
                VendorProjection.HardResync(projection, @event);
            }
            await repository.Save();
        }
    }
}
