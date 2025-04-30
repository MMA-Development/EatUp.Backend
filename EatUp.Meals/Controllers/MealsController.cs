using EatUp.Meals.DTO;
using EatUp.Meals.Models;
using EatUp.Meals.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Meals.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MealsController(IMealService mealService): ControllerBase
    {

        [HttpPost("{vendorId:guid}")]
        public async Task<IActionResult> AddMeal([FromRoute] Guid vendorId, [FromBody] AddMealDTO meal)
        {
            try
            {
                return Ok(await mealService.AddMeal(vendorId, meal));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int skip = 0, int take = 10)
        {
            var meals = await mealService.GetPage(skip, take);
            return Ok(meals);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await mealService.Delete(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
