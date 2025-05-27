using EatUp.Meals.DTO;

namespace EatUp.Meals.Repositories
{
    public interface IRecommendationRepository
    {
        Task<PaginationResult<MealDTO>> GetRecommendedMealsForUser(Guid userId, int skip, int take);
    }
}