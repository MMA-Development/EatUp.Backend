namespace EatUp.Users.Models
{
    public class UserFavorite: BaseEntity
    {
        public UserFavorite()
        {
            
        }
        public UserFavorite(Guid userId, Guid mealId)
        {
            MealId = mealId;
            UserId = userId;
        }
        public Guid MealId { get; set; }
        public Guid UserId { get; set; }
    }
}
