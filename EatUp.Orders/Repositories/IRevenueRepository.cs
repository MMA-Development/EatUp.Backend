
namespace EatUp.Orders.Repositories
{
    public interface IRevenueRepository
    {
        Task<object> GetRevenueByDateBetween(Guid vendorId, DateTime from, DateTime to);
        Task<double> GetTotalRevenueForVendor(Guid vendorId);
    }
}