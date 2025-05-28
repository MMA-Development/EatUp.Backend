using System.ComponentModel.DataAnnotations;
using EatUp.Vendors.Models;
using Microsoft.AspNetCore.Identity;

namespace EatUp.Vendors.DTO
{
    public class AddVendorDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string CVR { get; set; } = null!;

        [Required]
        public double Longitude { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public double Latitude { get; set; }

        public string Logo { get; set; }

        public Vendor ToVendor()
        {
            var vendor = new Vendor
            {
                Name = Name,
                Username = Username,
                Cvr = CVR,
                Location = new NetTopologySuite.Geometries.Point(Longitude, Latitude)
                {
                    SRID = 4326
                },
                Logo = Logo,
                Email = Email,
            };
            var hasher = new PasswordHasher<Vendor>();
            vendor.Password = hasher.HashPassword(vendor, Password);
            
            return vendor;
        }
    }
}
