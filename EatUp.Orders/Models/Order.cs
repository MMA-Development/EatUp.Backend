namespace EatUp.Orders.Models
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid FoodPackageId { get; set; }
        public string FoodPackageTitle { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public string? PaymentId { get; set; }
        public float Price { get; internal set; }
        public string StripeCustomerId { get; set; }
    }

    public enum PaymentStatusEnum
    {
        Pending = 0,
        Completed,
        Failed
    }
}
