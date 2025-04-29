using EatUp.Meals.Models;

namespace EatUp.Meals.Services
{
    public interface IMealService
    {
        void AddMeal(Meal meal);
        Task Delete(Guid id);
        void EnsureMeal(Meal meal);
        Task<PaginationResult<Meal>> GetPage(int skip, int take);
    }
}