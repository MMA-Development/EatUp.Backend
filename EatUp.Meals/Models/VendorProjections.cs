using EatUp.RabbitMQ.Events;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace EatUp.Meals.Models
{
    public class VendorProjections: BaseEntity
    {
        public string Name { get; set; }

        public Point Location { get; set; }

        //public static VendorProjections FromVendorCreatedEvent(VendorCreatedEvent vendor)
        //{
        //    return new VendorProjections
        //    {
        //        Id = vendor.Id,
        //        Name = vendor.Name,
        //        Location = new Point(vendor.Longitude, vendor.Longitude)
        //        {
        //            SRID = 4326
        //        }
        //    };
        //}
    }
}
