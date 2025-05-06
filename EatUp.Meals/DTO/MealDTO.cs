using System.Linq.Expressions;
using EatUp.Meals.Models;

namespace EatUp.Meals.DTO
{
    public class MealDTO
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public string VendorName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public float OriginalPrice { get; set; }
        public float Price { get; set; }
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }
        public int MaxOrderQuantity { get; set; }
        public DateTime FirstAvailablePickup { get; set; }
        public DateTime LastAvailablePickup { get; set; }
        public virtual List<Category> Categories { get; set; } = [];

        public static Expression<Func<Meal, MealDTO>> FromMeal = (meal) => new MealDTO
        {
            Id = meal.Id,
            VendorId = meal.VendorId,
            VendorName = meal.VendorName,
            Title = meal.Title,
            OriginalPrice = meal.OriginalPrice,
            Price = meal.Price,
            Description = meal.Description,
            Quantity = meal.Quantity,
            Categories = meal.Categories,
            FirstAvailablePickup = meal.FirstAvailablePickup,
            LastAvailablePickup = meal.LastAvailablePickup,
            MaxOrderQuantity = meal.MaxOrderQuantity
        };
    }
}
