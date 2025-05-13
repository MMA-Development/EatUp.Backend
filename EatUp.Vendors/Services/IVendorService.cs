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
        Task<PaginationResult<VendorDTO>> GetPage(VendorSearchParams @params);
        Task UpdateVendor(UpdateVendorDTO vendorDTO, Guid vendorId);
        Task<VendorDTO> GetVendorById(Guid vendorId);
        Task<VendorTokens> RefreshToken(string refreshToken);
        Task SignOut(string refreshToken);
    }
}