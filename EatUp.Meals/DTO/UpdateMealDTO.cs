using EatUp.Meals.Models;

namespace EatUp.Meals.DTO
{
    public class UpdateMealDTO
    {
        public string Title { get; set; } = null!;
        public float OriginalPrice { get; set; }
        public float Price { get; set; }
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }
        public int MaxOrderQuantity { get; set; }
        public DateTime FirstAvailablePickup { get; set; }
        public DateTime LastAvailablePickup { get; set; }

        public void MergeMeal(Meal meal)
        {
            meal.Title = Title;
            meal.OriginalPrice = OriginalPrice;
            meal.Price = Price;
            meal.Description = Description;
            meal.Quantity = Quantity;
            meal.MaxOrderQuantity = MaxOrderQuantity;
            meal.FirstAvailablePickup = FirstAvailablePickup;
            meal.LastAvailablePickup = LastAvailablePickup;
        }
    }
}
