using EatUp.RabbitMQ.Events.Order;

namespace EatUp.Meals.Models
{
    public class CompletedOrderProjection: BaseEntity
    {
        public Guid VendorId { get; set; }

        public Guid UserId { get; set; }

        public Guid MealId { get; set; }

        public int Quantity { get; set; }

        internal static CompletedOrderProjection FromEvent(OrderCompletedEvent @event)
        {
            return new CompletedOrderProjection
            {
                Id = @event.Id,
                VendorId = @event.VendorId,
                UserId = @event.UserId,
                MealId = @event.MealId,
                Quantity = @event.Quantity
            };
        }
    }
}
