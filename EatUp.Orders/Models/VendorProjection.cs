using EatUp.RabbitMQ.Events.Vendor;
using System.ComponentModel.DataAnnotations.Schema;

namespace EatUp.Orders.Models
{
    public class VendorProjection: BaseEntity
    {
        public string Name { get; set; } = null!;
        public static void Merge(VendorProjection projection, VendorUpdatedEvent vendor)
        {
            projection.Name = vendor.Name;
        }
        public static VendorProjection FromVendorCreatedEvent(VendorCreatedEvent vendor)
        {
            return new VendorProjection
            {
                Id = vendor.Id,
                Name = vendor.Name,
            };
        }

        public static VendorProjection FromVendorHardResyncEvent(VendorHardResyncEvent vendor) => new()
        {
            Id = vendor.Id,
            Name = vendor.Name,
            CreatedAt = vendor.CreatedAt,
            DeletedAt = vendor.DeletedAt,
            UpdatedAt = vendor.UpdatedAt
        };

        public static void HardResync(VendorProjection vendor, VendorHardResyncEvent @event)
        {
            vendor.Name = @event.Name;
            vendor.CreatedAt = @event.CreatedAt;
            vendor.DeletedAt = @event.DeletedAt;
            vendor.UpdatedAt = @event.UpdatedAt;
        }
    }
}
