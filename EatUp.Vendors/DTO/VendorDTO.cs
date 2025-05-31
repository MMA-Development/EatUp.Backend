using EatUp.Vendors.Models;
using System.Linq.Expressions;

namespace EatUp.Vendors.DTO
{
    public class VendorDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Logo { get; set; } = null!;
        public string Cvr { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        internal static Expression<Func<Vendor, VendorDTO>> FromVendor = (Vendor vendor) =>
            new VendorDTO
            {
                Name = vendor.Name,
                Logo = vendor.Logo,
                Cvr = vendor.Cvr,
                Username = vendor.Username,
                Email = vendor.Email,
                Longitude = vendor.Location.X,
                Latitude = vendor.Location.Y,
                Id = vendor.Id,
            };
    }
}
