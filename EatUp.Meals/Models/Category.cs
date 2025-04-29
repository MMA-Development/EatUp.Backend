namespace EatUp.Meals.Models
{
    public class Category: BaseEntity
    {
        public string Name { get; set; } = null!;
        
        public virtual List<Meal> Meals { get; set; } = [];
    }
}
