namespace EatUp.Orders.Models
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid VendorId { get; set; }
        public Guid FoodPackageId { get; set; }
        public string FoodPackageTitle { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public string? PaymentId { get; set; }
        public double Price { get; set; }
        public string StripeCustomerId { get; set; }
        public int Quantity { get; set; }
        public string VendorName { get; set; }
        public string? EphemeralKey { get; set; }
        public string? PaymentSecret { get; set; }
        public double OriginalPrice { get; internal set; }
    }

    public enum PaymentStatusEnum
    {
        Pending = 0,
        Completed,
        Failed,
        PickedUp,
        Created
    }
}
