using EatUp.Orders.DTO;
using EatUp.Orders.Models;
using Stripe;

namespace EatUp.Orders.Services
{
    public interface IOrderService
    {
        Task<object> CreateOrderRequest(CreateOrderRequest request);
        Task Delete(Guid id);
        Task<PaginationResult<Order>> GetPage(int skip, int take);
        Task HandlePaymentIntentSucceeded(PaymentIntent? paymentIntent);
        Task HandlePaymentIntentFailed(PaymentIntent? paymentMethod);
    }
}