using EatUp.Vendors.Models;

namespace EatUp.Vendors.DTO
{
    public class VendorDTO
    {
        public string Name { get; set; } = null!;
        public string Logo { get; set; } = null!;
        public string Cvr { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        internal static VendorDTO FromVendor(Vendor vendor)
        {
            return new VendorDTO
            {
                Name = vendor.Name,
                Logo = vendor.Logo,
                Cvr = vendor.Cvr,
                Username = vendor.Username,
                Email = vendor.Email,
                Longitude = vendor.Longitude,
                Latitude = vendor.Latitude
            };
        }
    }
}
