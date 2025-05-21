using EatUp.RabbitMQ.Events.Meals;

namespace EatUp.Orders.Models
{
    public class MealProjection: BaseEntity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        internal static MealProjection FromCreatedEvent(MealCreatedEvent @event)
        {
            throw new NotImplementedException();
        }

        internal static MealProjection FromHardResyncEvent(MealHardResyncEvent @event) => new()
        {
            Title = @event.Title,
            Description = @event.Description,
            CreatedAt = @event.CreatedAt,
            DeletedAt = @event.DeletedAt,
            Id = @event.Id,
            UpdatedAt = @event.UpdatedAt,
        };

        internal static void HardResync(MealProjection existing, MealHardResyncEvent @event)
        {
            existing.Title = @event.Title;
            existing.Description = @event.Description;
            existing.UpdatedAt = @event.UpdatedAt;
            existing.DeletedAt = @event.DeletedAt;
            existing.CreatedAt = @event.CreatedAt;
        }

        internal static void Update(MealProjection existing, MealUpdatedEvent @event)
        {
            existing.Title = @event.Title;
            existing.Description = @event.Description;
        }
    }
}
