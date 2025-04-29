namespace EatUp.Vendors.Models
{
    public class Category: BaseEntity
    {
        public string Name { get; set; } = null!;
        
        public virtual List<Vendor> Vendors { get; set; } = [];
    }
}
