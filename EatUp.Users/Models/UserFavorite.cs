namespace EatUp.Users.Models
{
    public class UserFavorite: BaseEntity
    {
        public Guid MealId { get; set; }
        public Guid UserId { get; set; }
    }
}
