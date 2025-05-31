namespace EatUp.Meals.DTO
{
    public class MealSearchParamsDTO
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string? Search { get; set; }
        public Guid[]? Categories { get; set; }
        public string? SortBy { get; set; }
        public bool Ascending { get; set; }
        public Guid? VendorId { get;  set; }
    }
}
