using EatUp.Meals.DTO;
using EatUp.Meals.Models;

namespace EatUp.Meals.Services
{
    public interface IMealService
    {
        Task<Guid> AddMeal(Guid vendorId, AddMealDTO meal);
        Task AddReview(Guid mealId, AddReviewDTO review, Guid userId);
        Task Delete(Guid mealId, Guid vendorId);
        Task<PaginationResult<MealDTO>> GetFavorites(Guid value, int skip, int take);
        Task<MealDTO> GetMeal(Guid mealId);
        Task<PaginationResult<MealDTO>> GetPage(MealSearchParamsDTO mealSearchParams);
        Task UpdateMeal(Guid mealId, Guid vendorId, UpdateMealDTO meal);
    }
}