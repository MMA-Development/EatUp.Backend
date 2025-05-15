using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;
using NetTopologySuite.Geometries;

namespace EatUp.Meals.EventHandlers
{
    public class VendorCreatedEventHandler(IRepository<VendorProjection> repository) : IEventHandler<VendorCreatedEvent>
    {
        public async Task HandleAsync(VendorCreatedEvent @event)
        {
            try
            {
                var vendor = VendorProjection.FromVendorCreatedEvent(@event);
                await repository.Insert(vendor);
                await repository.Save();
                Console.WriteLine($"Vendor Created: {@event.Id}, {@event.Name}, {@event.Email}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
