using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Orders.EventHandlers
{
    public class VendorDeletedEventHandler(IBaseRepository<VendorProjection> repository): IEventHandler<VendorDeletedEvent>
    {
        public async Task HandleAsync(VendorDeletedEvent @event)
        {
            await repository.Delete(@event.Id);
            await repository.Save();
        }
    }
}
