using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace EatUp.Vendors.Models
{
    public class Vendor: BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Logo { get; set; } = null!;
        public string Cvr { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Point? Location { get; set; }
        public string StripeAccountId { get; set; } = null!;

        [ForeignKey("VendorId")]
        public virtual List<RefreshTokenInformation> RefreshTokens { get; set; }
    }
}
