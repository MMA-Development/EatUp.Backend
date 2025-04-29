using EatUp.Vendors.Models;
using EatUp.Vendors.Repositories;

namespace EatUp.Vendors.Services
{
    public class Vendorservice(IBaseRepository<Vendor> repository) : IVendorservice
    {
        public void AddVendor(Vendor meal)
        {
            EsnureVendor(meal);
            repository.Insert(meal);
            repository.Save();
        }

        public async Task<PaginationResult<Vendor>> GetPage(int skip, int take)
        {
            return await repository.GetPage(skip, take, null, false);
        }

        public void EsnureVendor(Vendor meal)
        {
           
        }

        public async Task Delete(Guid id)
        {
            await repository.Delete(id);
            await repository.Save();
        }
    }
}
