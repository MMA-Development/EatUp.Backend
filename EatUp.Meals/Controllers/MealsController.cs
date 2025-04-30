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
        public async Task<IActionResult> GetPage([FromQuery] MealSearchParamsDTO mealSearchParams)
        {
            var meals = await mealService.GetPage(mealSearchParams);
            return Ok(meals);
        }

        [HttpPut("{mealId:guid}")]
        public async Task<IActionResult> UpdateMeal([FromRoute] Guid mealId, [FromQuery] Guid vendorId, [FromBody] UpdateMealDTO meal)
        {
            try
            {
                await mealService.UpdateMeal(mealId, vendorId, meal);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{mealId:guid}")]
        public async Task<IActionResult> GetMeal([FromRoute] Guid mealId)
        {
            try
            {
                return Ok(await mealService.GetMeal(mealId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{mealId:guid}")]
        public async Task<IActionResult> Delete(Guid vendorId, Guid mealId)
        {
            try
            {
                await mealService.Delete(mealId, vendorId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
