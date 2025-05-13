using EatUp.Orders.DTO;
using EatUp.Orders.Models;
using Stripe;

namespace EatUp.Orders.Services
{
    public interface IOrderService
    {
        Task<object> CreateOrderRequest(CreateOrderRequest request);
        Task Delete(Guid id);
        Task<PaginationResult<OrderDTO>> GetPageForVendor(OrdersForVendorParams @params, Guid vendorId);
        Task HandlePaymentIntentSucceeded(PaymentIntent? paymentIntent);
        Task HandlePaymentIntentFailed(PaymentIntent? paymentMethod);
        Task<PaginationResult<OrderDTO>> GetPageForUser(int skip, int take, Guid vendorId);
    }
}