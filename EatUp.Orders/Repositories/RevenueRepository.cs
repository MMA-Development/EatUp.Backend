using EatUp.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EatUp.Orders.Repositories
{
    public class RevenueRepository(Context context) : IRevenueRepository
    {
        public async Task<double> GetTotalRevenueForVendor(Guid vendorId)
        {
            return await context.GetQuery<Order>()
                .Where(o => o.VendorId == vendorId && o.PaymentStatus == PaymentStatusEnum.Completed)
                .SumAsync(o => o.Price);
        }

        public async Task<object> GetRevenueByDateBetween(Guid vendorId, DateTime from, DateTime to)
        {
            var query = context.GetQuery<Order>().Where(o => (o.PaymentStatus == PaymentStatusEnum.Completed || o.PaymentStatus == PaymentStatusEnum.PickedUp) && o.UpdatedAt >= from && o.UpdatedAt <= to && o.VendorId == vendorId)
                .GroupBy(o => o.UpdatedAt.Value.Date, x => x)
                .Select(x => new
                {
                    Date = x.Key,
                    Revenue = x.Sum(o => o.Price),
                    Orders = x.Count(),
                });

            return await query.ToListAsync();
        }
    }
}
