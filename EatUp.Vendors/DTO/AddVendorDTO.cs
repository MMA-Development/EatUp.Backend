using EatUp.Vendors.Models;
using Microsoft.AspNetCore.Identity;

namespace EatUp.Vendors.DTO
{
    public class AddVendorDTO
    {
        public string Name { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string CVR { get; set; } = null!;

        public double Longitude { get; set; }

        public string Email { get; set; }

        public double Latitude { get; set; }

        public Vendor ToVendor()
        {
            var vendor = new Vendor
            {
                Name = Name,
                Username = Username,
                Cvr = CVR,
                Longitude = Longitude,
                Latitude = Latitude,
                Logo = "default",
            };
            var hasher = new PasswordHasher<Vendor>();
            vendor.Password = hasher.HashPassword(vendor, Password);
            
            return vendor;
        }
    }
}
