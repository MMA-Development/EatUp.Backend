using EatUp.Meals.DTO;
using EatUp.Meals.Models;
using EatUp.Meals.Repositories;

namespace EatUp.Meals.Services
{
    public class RecommendationService(IRecommendationRepository repository) : IRecommendationService
    {
        public async Task<PaginationResult<MealDTO>> GetRecommendedMeals(Guid userId, int skip, int take)
        {
            return await repository.GetRecommendedMealsForUser(userId, skip, take);
        }
    }
}
