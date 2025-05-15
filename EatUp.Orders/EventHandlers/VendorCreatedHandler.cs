using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Orders.EventHandlers
{
    public class VendorCreatedHandler : IEventHandler<VendorCreatedEvent>
    {
        public Task HandleAsync(VendorCreatedEvent @event)
        {
            Console.WriteLine($"Vendor Created: {@event.Id}, {@event.Name}, {@event.Email}");

            return Task.CompletedTask;
        }
    }
}
