namespace EatUp.Vendors.Models
{
    public class Vendor: BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Logo { get; set; } = null!;
        public string Cvr { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string StripeAccountId { get; set; } = null!;
    }
}
