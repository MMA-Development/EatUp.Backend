using System.Text.Json;
using EatUp.Vendors.DTO;
using EatUp.Vendors.Models;
using EatUp.Vendors.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Swashbuckle.AspNetCore.Annotations;

namespace EatUp.Vendors.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorsController(IVendorservice vendorService) : ControllerBase
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

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInVendorDTO signInVendor)
        {
            try
            {
                return Ok(await vendorService.SignIn(signInVendor));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{vendorId:guid}")]
        public async Task<IActionResult> UpdateVendor([FromBody] UpdateVendorDTO vendorDTO, [FromRoute] Guid vendorId)
        {
            try
            {
                await vendorService.UpdateVendor(vendorDTO, vendorId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPage(int skip = 0, int take = 10)
        {
            var Vendors = await vendorService.GetPage(skip, take);
            return Ok(Vendors);
        }

        [HttpGet("{vendorId:guid}")]
        public async Task<IActionResult> GetVendor([FromRoute] Guid vendorId)
        {
            try
            {
                VendorDTO vendor = await vendorService.GetVendorById(vendorId);
                return Ok(vendor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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
