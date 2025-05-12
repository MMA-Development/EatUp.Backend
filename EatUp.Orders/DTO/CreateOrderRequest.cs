using EatUp.Orders.Models;

namespace EatUp.Orders.DTO
{
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid FoodPackageId { get; set; }
        public float Price { get; set; }
        public string FoodPackageTitle { get; set; }
        public string StripeCustomerId { get; set; }
        public Guid VendorId { get; set; }

        public Order ToOrder()
        {
            return new Order
            {
                UserId = UserId,
                UserName = UserName,
                FoodPackageId = FoodPackageId,
                FoodPackageTitle = FoodPackageTitle,
                PaymentStatus = PaymentStatusEnum.Pending,
                Price = Price,
                StripeCustomerId = StripeCustomerId,
                VendorId = VendorId
            };
        }
    }
}
