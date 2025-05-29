using System.ComponentModel.DataAnnotations.Schema;

namespace EatUp.Meals.Models
{
    public class Meal: BaseEntity
    {
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
        public string ImageUrl { get; internal set; }

        [ForeignKey("MealId")]
        public virtual List<CompletedOrderProjection> CompletedOrders { get; set; }

        [ForeignKey("MealId")]
        public virtual List<Review> Reviews { get; set; }
    }
}
