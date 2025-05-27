using EatUp.Meals.DTO;

namespace EatUp.Meals.Services
{
    public interface IRecommendationService
    {
        Task<PaginationResult<MealDTO>> GetRecommendedMeals(Guid userId, int skip, int take);
    }
}