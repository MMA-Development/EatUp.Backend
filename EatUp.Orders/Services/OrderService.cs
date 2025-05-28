using EatUp.Orders.DTO;
using EatUp.Orders.Extensions;
using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.RabbitMQ.Events.Order;
using Stripe;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EatUp.Orders.Services
{
    public class OrderService(IBaseRepository<Order> repository,
        IBaseRepository<MealProjection> mealProjections,
        IBaseRepository<UserProjection> userProjections,
        IBaseRepository<VendorProjection> vendorProjections,
        IRabbitMqPublisher publisher) : IOrderService
    {
        public async Task<PaginationResult<OrderDTO>> GetPageForVendor(OrdersForVendorParams @params, Guid vendorId)
        {
            var expression = GetExpression(@params, vendorId);
            return await repository.GetPage(@params.Skip, @params.Take, OrderDTO.FromEntity, expression);
        }
        public async Task<PaginationResult<OrderDTO>> GetPageForUser(int skip, int take, Guid userId)
        {
            Expression<Func<Order, bool>> expression = x => x.UserId == userId;
            return await repository.GetPage(skip, take, OrderDTO.FromEntity, expression);
        }

        private Expression<Func<Order, bool>> GetExpression(OrdersForVendorParams @params, Guid vendorId)
        {
            List<Expression<Func<Order, bool>>> expressions = new List<Expression<Func<Order, bool>>>();

            expressions.Add(order => order.VendorId == vendorId);

            if (!string.IsNullOrEmpty(@params.Search))
            {
                expressions.Add(order => order.UserName.Contains(@params.Search) || order.Id.ToString().Contains(@params.Search));
            }

            return expressions.AndAll();
        }

        private void EnsureOrder(CreateOrderRequest order, MealProjection meal, VendorProjection vendor, UserProjection user)
        {
            ArgumentNullException.ThrowIfNull(order);
            ArgumentNullException.ThrowIfNull(meal);
            ArgumentNullException.ThrowIfNull(vendor);
            ArgumentNullException.ThrowIfNull(user);
        }
        public async Task Delete(Guid id)
        {
            await repository.Delete(id);
            await repository.Save();
        }

        public async Task<object> CreateOrderRequest(Guid userId, CreateOrderRequest request)
        {
            var meal = await mealProjections.GetById(request.FoodPackageId);
            var vendor = await vendorProjections.GetById(request.VendorId);
            var user = await userProjections.GetById(userId);

            EnsureOrder(request, meal, vendor, user);
            EnsureMealIsAvailable(request, meal);

            var order = await repository.GetByExpression(order => order.UserId == userId && order.PaymentStatus == PaymentStatusEnum.Pending);

            if (order == null)
            {
                order = request.ToOrder(meal.Title, user, vendor.Name);
                await repository.Insert(order);
                await repository.Save();
                PaymentIntent paymentIntent = await CreatePaymentIntentForOrder(order);
                EphemeralKey ephemeralKey = await CreateEphemeralKey(order.StripeCustomerId);
                order.PaymentId = paymentIntent.Id;
                order.EphemeralKey = ephemeralKey.Secret;
                order.PaymentSecret = paymentIntent.ClientSecret;
            }
            else
            {
                request.Merge(order, meal.Title, user, vendor.Name);
                await UpdatePaymentIntent(order);
            }

            await repository.Save();

            return new
            {
                ephemeralKey = order.EphemeralKey,
                clientSecret = order.PaymentSecret,
                orderId = order.Id
            };
        }

        private void EnsureMealIsAvailable(CreateOrderRequest orderRequest, MealProjection meal)
        {
            var ordersForMeal = repository.Query(x => x.FoodPackageId == orderRequest.FoodPackageId && x.VendorId == orderRequest.VendorId && x.PaymentStatus == PaymentStatusEnum.Completed);
            var totalBought = ordersForMeal.Sum(x => x.Quantity);
            if (meal.Quantity < totalBought + orderRequest.Quantity)
            {
                throw new InvalidOperationException("Meal is not available in the requested quantity.");
            }
        }

        private async Task UpdatePaymentIntent(Order order)
        {
            var service = new PaymentIntentService();
            await service.UpdateAsync(order.PaymentId, new PaymentIntentUpdateOptions
            {
                Amount = (long)order.Price * 100,
            });
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
            if (paymentIntent.Metadata.TryGetValue("order_id", out string? orderId))
            {
                var order = await repository.GetById(Guid.Parse(orderId), true);
                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                order.PaymentStatus = PaymentStatusEnum.Completed;
                await repository.Save();

                var @event = ToCompletedOrderEvent(order);
                await publisher.Publish(@event);
            }
            else
            {
                throw new ArgumentException("Order ID not found in payment intent metadata");
            }
        }

        private static OrderCompletedEvent ToCompletedOrderEvent(Order order) => new()
        {
            Id = order.Id,
            MealId = order.FoodPackageId,
            Quantity = order.Quantity,
            UserId = order.UserId,
            VendorId = order.VendorId
        };

        public async Task HandlePaymentIntentFailed(PaymentIntent? paymentIntent)
        {
            _ = paymentIntent ?? throw new ArgumentNullException();
            if (paymentIntent.Metadata.TryGetValue("order_id", out string? orderId))
            {
                var order = await repository.GetById(Guid.Parse(orderId), true);
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
