using EatUp.Vendors.Models;
using EatUp.Vendors.Services;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Vendors.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorsController(IVendorservice vendorService): ControllerBase
    {

        [HttpPost]
        public IActionResult AddVendor([FromBody] Vendor vendor)
        {
            try
            {
                vendorService.AddVendor(vendor);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int skip = 0, int take = 10)
        {
            var Vendors = await vendorService.GetPage(skip, take);
            return Ok(Vendors);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await vendorService.Delete(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
