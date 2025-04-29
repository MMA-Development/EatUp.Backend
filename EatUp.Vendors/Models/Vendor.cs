namespace EatUp.Vendors.Models
{
    public class Vendor: BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Logo { get; set; } = null!;
        public string Cvr { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string StripeAccountId { get; set; } = null!;
    }
}
