using EatUp.Orders.Models;
using EatUp.Orders.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Orders.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController(IOrderService orderService): ControllerBase
    {

        [HttpPost]
        public IActionResult AddMeal([FromBody] Order order)
        {
            try
            {
                orderService.AddOrder(order);
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
            var meals = await orderService.GetPage(skip, take);
            return Ok(meals);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await orderService.Delete(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
