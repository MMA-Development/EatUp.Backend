using EatUp.Meals.DTO;
using EatUp.Meals.Models;

namespace EatUp.Meals.Services
{
    public interface IMealService
    {
        Task<Guid> AddMeal(Guid vendorId, AddMealDTO meal);
        Task Delete(Guid mealId, Guid vendorId);
        void EnsureMeal(Meal meal);
        Task<MealDTO> GetMeal(Guid mealId);
        Task<PaginationResult<MealDTO>> GetPage(MealSearchParamsDTO mealSearchParams);
        Task UpdateMeal(Guid mealId, Guid vendorId, UpdateMealDTO meal);
    }
}