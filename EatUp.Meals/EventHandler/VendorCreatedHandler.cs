using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events;
using NetTopologySuite.Geometries;

namespace EatUp.Meals.EventHandler
{
    public class VendorCreatedHandler(IRepository<VendorProjections> repository) : IEventHandler<VendorCreatedEvent>
    {
        public async Task HandleAsync(VendorCreatedEvent @event)
        {
            try
            {
                var vendor = new VendorProjections
                {
                    Id = @event.Id,
                    Name = @event.Name,
                    Location = new Point(@event.Longitude, @event.Longitude)
                    {
                        SRID = 4326
                    }
                };
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
