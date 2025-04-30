using System.ComponentModel.DataAnnotations.Schema;

namespace EatUp.Vendors.Models
{
    public class RefreshTokenInformation: BaseEntity
    {
        public string RefreshToken { get; set; } = null!;
        
        public Guid VendorId { get; set; }

        public Vendor Vendor { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
