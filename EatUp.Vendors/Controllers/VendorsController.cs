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

        [HttpPost("signout")]
        public async Task<IActionResult> SignOut([FromBody] string refreshToken)
        {
            try
            {
                await vendorService.SignOut(refreshToken);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "Vendor")]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateVendor([FromBody] UpdateVendorDTO vendorDTO, [FromHeader] Guid vendorId)
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
        public async Task<IActionResult> GetPage([FromQuery] VendorSearchParams @params)
        {
            var Vendors = await vendorService.GetPage(@params);
            return Ok(Vendors);
        }

        [HttpGet("{vendorId:guid}")]
        [AllowAnonymous]
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


        [HttpGet("me")]
        public async Task<IActionResult> GetMe([FromHeader] Guid vendorId)
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

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] string refreshToken)
        {
            try
            {
                VendorTokens token = await vendorService.RefreshToken(refreshToken);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("me")]
        public async Task<IActionResult> Delete([FromHeader] Guid vendorId)
        {
            try
            {
                await vendorService.Delete(vendorId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
