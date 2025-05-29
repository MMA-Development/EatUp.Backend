using EatUp.Meals.Models;

namespace EatUp.Meals.DTO
{
    public class AddReviewDTO
    {
        public double Rating { get; set; }
        public string? Description { get; set; }

        public Review ToReview(Guid mealId, Guid userId) => new()
        {
            Rating = Rating,
            Description = Description,
            MealId = mealId,
            UserId = userId
        };
    }
}
