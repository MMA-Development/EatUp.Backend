using EatUp.Orders.Models;
using EatUp.Orders.Repositories;

namespace EatUp.Orders.Services
{
    public class OrderService(IBaseRepository<Order> repository) : IOrderService
    {
        public void AddMeal(Order meal)
        {
            EnsureOrder(meal);
            repository.Insert(meal);
            repository.Save();
        }

        public async Task<PaginationResult<Order>> GetPage(int skip, int take)
        {
            return await repository.GetPage(skip, take, null, false);
        }

        public void EnsureOrder(Order meal)
        {
           
        }

        public async Task Delete(Guid id)
        {
            await repository.Delete(id);
            await repository.Save();
        }
    }
}
