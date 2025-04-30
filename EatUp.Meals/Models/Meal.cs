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
    }
}
