using EatUp.RabbitMQ.Events.Vendor;

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
    }
}
