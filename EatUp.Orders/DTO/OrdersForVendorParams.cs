namespace EatUp.Orders.DTO
{
    public class OrdersForVendorParams
    {
        public int Take { get; set; }

        public int Skip { get; set; }

        public string? Search { get; set; }
    }
}
