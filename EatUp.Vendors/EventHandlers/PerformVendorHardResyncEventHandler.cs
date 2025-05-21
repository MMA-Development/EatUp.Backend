using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;
using EatUp.Vendors.Models;
using EatUp.Vendors.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EatUp.Vendors.EventHandlers
{
    public class PerformVendorHardResyncEventHandler(IRabbitMqPublisher publisher, IRepository<Vendor> repository) : IEventHandler<PerformVendorHardResyncEvent>
    {
        public async Task HandleAsync(PerformVendorHardResyncEvent _)
        {
            var vendors = await (await repository.GetAll()).ToArrayAsync();
            
            foreach (Vendor vendor in vendors)
            {
                var @event = ToEvent(vendor);
                await publisher.Publish(@event);
            }
        }

        private VendorHardResyncEvent ToEvent(Vendor vendor) => new()
        {
            Id = vendor.Id,
            Name = vendor.Name,
            Latitude = vendor.Location?.X ?? 0,
            Longitude = vendor.Location?.Y ?? 0,
            CreatedAt = vendor.CreatedAt,
            DeletedAt = vendor.DeletedAt,
            UpdatedAt = vendor.UpdatedAt
        };
    }
}
