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
    }
}
