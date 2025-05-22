using EatUp.Orders.Models;

namespace EatUp.Orders.DTO
{
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public Guid FoodPackageId { get; set; }
        public float Price { get; set; }
        public Guid VendorId { get; set; }
        public int Quantity { get; set; }

        public Order ToOrder(string mealTitle, UserProjection user, string vendorName)
        {
            return new Order
            {
                UserId = UserId,
                UserName = user.Fullname,
                FoodPackageId = FoodPackageId,
                FoodPackageTitle = mealTitle,
                VendorName = vendorName,
                PaymentStatus = PaymentStatusEnum.Pending,
                Price = Price,
                StripeCustomerId = user.StripeCustomerId,
                VendorId = VendorId,
                Quantity = Quantity,
            };
        }
    }
}
