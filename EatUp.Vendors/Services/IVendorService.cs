using EatUp.Vendors.DTO;
using EatUp.Vendors.Models;
using Stripe;

namespace EatUp.Vendors.Services
{
    public interface IVendorservice
    {
        Task<AccountLink?> AddVendor(AddVendorDTO addVendor);
        Task<VendorTokens> SignIn(SignInVendorDTO addVendor);
        Task Delete(Guid id);
        Task<PaginationResult<Vendor>> GetPage(int skip, int take);
    }
}