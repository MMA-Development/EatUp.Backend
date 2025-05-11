using EatUp.Orders.DTO;
using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using Stripe;

namespace EatUp.Orders.Services
{
    public class OrderService(IBaseRepository<Order> repository) : IOrderService
    {
        public async Task<PaginationResult<Order>> GetPage(int skip, int take)
        {
            return await repository.GetPage(skip, take, null, false);
        }
        public void EnsureOrder(CreateOrderRequest order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            if (order.UserId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty", nameof(order.UserId));
            }
            if (order.FoodPackageId == Guid.Empty)
            {
                throw new ArgumentException("FoodPackageId cannot be empty", nameof(order.FoodPackageId));
            }
        }
        public async Task Delete(Guid id)
        {
            await repository.Delete(id);
            await repository.Save();
        }

        public async Task<object> CreateOrderRequest(CreateOrderRequest request)
        {
            EnsureOrder(request);
            var order = request.ToOrder();
            await repository.Insert(order);
            await repository.Save();
            
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(order.Price * 100),
                Currency = "dkk",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", order.Id.ToString() }
                }
            };

            var paymentIntent = await paymentIntentService.CreateAsync(options);
            order.PaymentId = paymentIntent.Id;
            await repository.Save();

            return new
            {
                clientSecret = paymentIntent.ClientSecret,
                orderId = order.Id
            };
        }
    }
}
