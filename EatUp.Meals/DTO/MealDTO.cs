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
        public string ImageUrl { get; set; }
        public DateTime FirstAvailablePickup { get; set; }
        public DateTime LastAvailablePickup { get; set; }
        public virtual List<CategoryDTO> Categories { get; set; } = [];
        public int Available { get; set; }
        public double? AverageReview { get; set; }

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
            Categories = meal.Categories.Select(x => new CategoryDTO { Id = x.Id, Name = x.Name}).ToList(),
            FirstAvailablePickup = meal.FirstAvailablePickup,
            LastAvailablePickup = meal.LastAvailablePickup,
            MaxOrderQuantity = meal.MaxOrderQuantity,
            Available = meal.Quantity - meal.CompletedOrders.Select(x => x.Quantity).Sum(),
            ImageUrl = meal.ImageUrl,
            AverageReview = meal.Reviews.Count > 0 ? meal.Reviews.Average(x => x.Rating) : null
        };
    }
}
