namespace EatUp.Users.Models
{
    public class OrderCompletedProjection: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public double TotalPrice { get; set; }
        public double OriginalPrice { get; set; }
        public int Quantity { get; set; }
    }
}
