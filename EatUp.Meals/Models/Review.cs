namespace EatUp.Meals.Models
{
    public class Review: BaseEntity
    {
        public double Rating { get; set; }
        public string? Description { get; set; }
        public Guid UserId { get; set; }
        public Guid MealId { get; set; }
    }
}
