namespace EatUp.Vendors.DTO
{
    public class VendorSearchParams
    {
        public int Take { get; set; } = 0;
        public int Skip { get; set; } = 0;
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string? Search { get; set; }
        public double Radius { get; set; }
    }
}
