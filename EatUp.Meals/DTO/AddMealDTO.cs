using EatUp.Meals.Models;

namespace EatUp.Meals.DTO
{
    public class AddMealDTO
    {
        public string Title { get; set; } = null!;
        public float OriginalPrice { get; set; }
        public float Price { get; set; }
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }
        public int MaxOrderQuantity { get; set; }

        public DateTime FirstAvailablePickup { get; set; }
        public DateTime LastAvailablePickup { get; set; }
        public Guid[] Categories { get; set; }

        internal Meal ToMeal(Guid vendorId, string vendorName, List<Category> categories)
        {
            return new Meal
            {
                VendorId = vendorId,
                VendorName = vendorName,
                Title = Title,
                OriginalPrice = OriginalPrice,
                Price = Price,
                Description = Description,
                Quantity = Quantity,
                MaxOrderQuantity = MaxOrderQuantity,
                FirstAvailablePickup = FirstAvailablePickup,
                LastAvailablePickup = LastAvailablePickup,
                Categories = categories,
            };
        }
    }
}
