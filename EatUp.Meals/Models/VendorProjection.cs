using EatUp.RabbitMQ.Events;
using EatUp.RabbitMQ.Events.Vendor;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace EatUp.Meals.Models
{
    public class VendorProjection : BaseEntity
    {
        public string Name { get; set; }

        public Point Location { get; set; }

        public static void Merge(VendorProjection projection, VendorUpdatedEvent vendor)
        {
            projection.Name = vendor.Name;
            projection.Location = new Point(vendor.Longitude, vendor.Latitude)
            {
                SRID = 4326
            };
        }

        public static VendorProjection FromVendorCreatedEvent(VendorCreatedEvent vendor)
        {
            return new VendorProjection
            {
                Id = vendor.Id,
                Name = vendor.Name,
                Location = new Point(vendor.Longitude, vendor.Longitude)
                {
                    SRID = 4326
                }
            };
        }

        internal static VendorProjection FromVendorHardResyncEvent(VendorHardResyncEvent @event) => new()
        {
            CreatedAt = @event.CreatedAt,
            DeletedAt = @event.DeletedAt,
            Id = @event.Id,
            Name = @event.Name,
            Location = new Point(@event.Longitude, @event.Latitude)
            {
                SRID = 4326
            },
            UpdatedAt = @event.UpdatedAt
        };

        internal static void HardResync(VendorProjection projection, VendorHardResyncEvent @event)
        {
            projection.Name = @event.Name;
            projection.Location = new Point(@event.Longitude, @event.Latitude)
            {
                SRID = 4326
            };
            projection.UpdatedAt = @event.UpdatedAt;
            projection.DeletedAt = @event.DeletedAt;
            projection.CreatedAt = @event.CreatedAt;
        }
    }
}
