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

            PaymentIntent paymentIntent = await CreatePaymentIntentForOrder(order);
            EphemeralKey ephermalKey = await CreateEphemeralKey(order.StripeCustomerId);

            order.PaymentId = paymentIntent.Id;
            await repository.Save();

            return new
            {
                ephermalKey = ephermalKey.Secret,
                clientSecret = paymentIntent.ClientSecret,
                orderId = order.Id
            };
        }

        private static async Task<EphemeralKey> CreateEphemeralKey(string customerId)
        {
            var ephermalKeyService = new EphemeralKeyService();
            var ephermalKey = await ephermalKeyService.CreateAsync(new EphemeralKeyCreateOptions
            {
                Customer = customerId,
            });
            return ephermalKey;
        }

        private static async Task<PaymentIntent> CreatePaymentIntentForOrder(Order order)
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(order.Price * 100),
                Currency = "dkk",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", order.Id.ToString() }
                },
                Customer = order.StripeCustomerId,
            };

            var paymentIntent = await paymentIntentService.CreateAsync(options);
            return paymentIntent;
        }

        public async Task HandlePaymentIntentSucceeded(PaymentIntent? paymentIntent)
        {
            _ = paymentIntent ?? throw new ArgumentNullException();
            if (paymentIntent.Metadata.TryGetValue("order_id", out string orderId))
            {
                var order = await repository.GetById(Guid.Parse(orderId));
                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                order.PaymentStatus = PaymentStatusEnum.Completed;
                await repository.Save();
            }
            else
            {
                throw new ArgumentException("Order ID not found in payment intent metadata");
            }
        }

        public async Task HandlePaymentIntentFailed(PaymentIntent? paymentIntent)
        {
            _ = paymentIntent ?? throw new ArgumentNullException();
            if (paymentIntent.Metadata.TryGetValue("order_id", out string orderId))
            {
                var order = await repository.GetById(Guid.Parse(orderId));
                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                order.PaymentStatus = PaymentStatusEnum.Failed;
                await repository.Save();
            }
            else
            {
                throw new ArgumentException("Order ID not found in payment intent metadata");
            }
        }
    }
}
