using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Meals;

namespace EatUp.Meals.EventHandlers
{
    public class PerformMealHardResyncEventHandler(IRepository<Meal> repository, IRabbitMqPublisher publisher) : IEventHandler<PerformMealHardResyncEvent>
    {
        public async Task HandleAsync(PerformMealHardResyncEvent _)
        {
            var allMeals = (await repository.GetAll()).ToArray();
            foreach (var meal in allMeals)
            {
                var @event = ToEvent(meal);
                await publisher.Publish(@event);
            }
        }

        private MealHardResyncEvent ToEvent(Meal meal) => new()
        {
            Id = meal.Id,
            Description = meal.Description,
            Price = meal.Price,
            VendorId = meal.VendorId,
            CreatedAt = meal.CreatedAt,
            UpdatedAt = meal.UpdatedAt,
            LastAvailablePickup = meal.LastAvailablePickup,
            DeletedAt = meal.DeletedAt,
            FirstAvailablePickup = meal.FirstAvailablePickup,
            MaxOrderQuantity = meal.MaxOrderQuantity,
            OriginalPrice = meal.OriginalPrice,
            Quantity = meal.Quantity,
            Title = meal.Title,
            VendorName = meal.VendorName,
        };
    }
}
