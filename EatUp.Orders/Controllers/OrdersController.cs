using EatUp.Orders.DTO;
using EatUp.Orders.Models;
using EatUp.Orders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Orders.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController(IOrderService orderService): ControllerBase
    {

        [HttpPost("request")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> CreateOrderRequest([FromBody] CreateOrderRequest request, [FromHeader] Guid userId)
        {
            try
            {
                request.UserId = userId;
                var result = await orderService.CreateOrderRequest(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetPage(int skip = 0, int take = 10)
        {
            var meals = await orderService.GetPage(skip, take);
            return Ok(meals);
        }

        [HttpDelete("{orderId:guid}/cancel")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Delete(Guid orderId, [FromHeader] Guid userId)
        {
            try
            {
                await orderService.Delete(orderId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
