using EatUp.Vendors.DTO;
using EatUp.Vendors.Models;
using EatUp.Vendors.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Swashbuckle.AspNetCore.Annotations;

namespace EatUp.Vendors.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorsController(IVendorservice vendorService): ControllerBase
    {

        [HttpPost("signup")]
        public async Task<IActionResult> AddVendor([FromBody] AddVendorDTO vendorDTO)
        {
            try
            {
                return Ok(await vendorService.AddVendor(vendorDTO));
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
