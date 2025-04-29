using EatUp.Orders.Models;

namespace EatUp.Orders.Services
{
    public interface IOrderService
    {
        void AddMeal(Order meal);
        Task Delete(Guid id);
        void EnsureOrder(Order meal);
        Task<PaginationResult<Order>> GetPage(int skip, int take);
    }
}