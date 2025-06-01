using EatUp.Orders.Models;

namespace EatUp.Orders.DTO
{
    public class CreateOrderRequest
    {
        public Guid FoodPackageId { get; set; }
        public int Quantity { get; set; }

        public Order ToOrder(MealProjection meal, UserProjection user, string vendorName)
        {
            return new Order
            {
                UserId = user.Id,
                UserName = user.Fullname,
                FoodPackageId = FoodPackageId,
                FoodPackageTitle = meal.Title,
                VendorName = vendorName,
                PaymentStatus = PaymentStatusEnum.Created,
                Price = meal.Price * Quantity,
                OriginalPrice = meal.OriginalPrice * Quantity,
                StripeCustomerId = user.StripeCustomerId,
                VendorId = meal.VendorId,
                Quantity = Quantity,
            };
        }

        internal void Merge(Order order, MealProjection meal, UserProjection user, string name)
        {
            order.FoodPackageTitle = meal.Title;
            order.VendorName = name;
            order.VendorId = meal.VendorId;
            order.FoodPackageId = FoodPackageId;
            order.Price = meal.Price * Quantity;
            order.OriginalPrice = meal.OriginalPrice * Quantity;
            order.Quantity = Quantity;
        }
    }
}
