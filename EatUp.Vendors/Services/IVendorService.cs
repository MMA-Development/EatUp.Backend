using EatUp.Vendors.Models;

namespace EatUp.Vendors.Services
{
    public interface IVendorservice
    {
        void AddVendor(Vendor meal);
        Task Delete(Guid id);
        Task<PaginationResult<Vendor>> GetPage(int skip, int take);
    }
}