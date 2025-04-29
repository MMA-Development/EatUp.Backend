namespace EatUp.Orders.Models
{
    public class Order: BaseEntity
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid FoodPackageId { get; set; }
        public string FoodPackageTitle { get; set; }
        public string PaymentStatus { get; set; } 
        public string PaymentId { get; set; } 
    }
}
