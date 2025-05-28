using EatUp.RabbitMQ.Events.Meals;

namespace EatUp.Orders.Models
{
    public class MealProjection : BaseEntity
    {
        public string Title { get; set; }

        public string Description { get; set; }
        public int Quantity { get; internal set; }

        internal static MealProjection FromCreatedEvent(MealCreatedEvent @event) => new()
        {
            Quantity = @event.Quantity,
            Description = @event.Description,
            Id = @event.Id,
            Title = @event.Title,
        };

        internal static MealProjection FromHardResyncEvent(MealHardResyncEvent @event) => new()
        {
            Title = @event.Title,
            Description = @event.Description,
            CreatedAt = @event.CreatedAt,
            DeletedAt = @event.DeletedAt,
            Id = @event.Id,
            UpdatedAt = @event.UpdatedAt,
            Quantity = @event.Quantity,
        };

        internal static void HardResync(MealProjection existing, MealHardResyncEvent @event)
        {
            existing.Title = @event.Title;
            existing.Description = @event.Description;
            existing.UpdatedAt = @event.UpdatedAt;
            existing.DeletedAt = @event.DeletedAt;
            existing.CreatedAt = @event.CreatedAt;
            existing.Quantity = @event.Quantity;
        }

        internal static void Update(MealProjection existing, MealUpdatedEvent @event)
        {
            existing.Title = @event.Title;
            existing.Description = @event.Description;
            existing.Quantity = @event.Quantity;
        }
    }
}
