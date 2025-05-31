using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Meals.Models
{
    public class UserFavoriteMealsProjection : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid MealId { get; set; }


        public static UserFavoriteMealsProjection FromEvent(UserFavoritedAMealEvent @event) => new()
        {
            Id = @event.Id,
            MealId = @event.MealId,
            UserId = @event.UserId, 
        };

        internal static UserFavoriteMealsProjection? FromEvent(UserFavoriteHardResyncEvent @event) => new UserFavoriteMealsProjection()
        {
            DeletedAt = @event.DeletedAt,
            Id = @event.Id,
            MealId = @event.MealId,
            UserId = @event.UserId,
        };


        internal static void MergeEvent(UserFavoriteMealsProjection existing, UserFavoriteHardResyncEvent @event)
        {
            existing.DeletedAt = @event.DeletedAt;
            existing.MealId = @event.MealId;
            existing.UserId = @event.UserId;
        }
    }
}
