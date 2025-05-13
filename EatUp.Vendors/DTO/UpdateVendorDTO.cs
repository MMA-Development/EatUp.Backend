using EatUp.Vendors.Models;

namespace EatUp.Vendors.DTO
{
    public class UpdateVendorDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        internal void Merge(Vendor vendorFromDb)
        {
            vendorFromDb.Email = Email;
            vendorFromDb.Name = Name;
            vendorFromDb.Location = new NetTopologySuite.Geometries.Point(Longitude, Latitude)
            {
                SRID = 4326
            };
        }
    }
}
