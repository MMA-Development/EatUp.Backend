using EatUp.Meals.DTO;
using EatUp.Meals.Models;
using EatUp.Meals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Meals.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MealsController(IMealService mealService) : EatUpController
    {
        [Authorize(Policy = "Vendor")]
        [HttpPost]
        public async Task<IActionResult> AddMeal([FromBody] AddMealDTO meal)
        {
            try
            {
                return Ok(await mealService.AddMeal(VendorId.Value, meal));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPage([FromQuery] MealSearchParamsDTO mealSearchParams)
        {
            try
            {
                var meals = await mealService.GetPage(mealSearchParams);
                return Ok(meals);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("vendor")]
        [Authorize(Policy = "Vendor")]
        public async Task<IActionResult> GetPageVendor([FromQuery] MealSearchParamsDTO mealSearchParams)
        {
            try
            {
                mealSearchParams.VendorId = VendorId.Value;
                var meals = await mealService.GetPage(mealSearchParams);
                return Ok(meals);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{mealId:guid}")]
        [Authorize(Policy = "Vendor")]
        public async Task<IActionResult> UpdateMeal([FromRoute] Guid mealId, [FromBody] UpdateMealDTO meal)
        {
            try
            {
                await mealService.UpdateMeal(mealId, VendorId.Value, meal);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{mealId:guid}")]
        [Authorize]
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
        [Authorize(Policy = "Vendor")]
        public async Task<IActionResult> Delete( Guid mealId)
        {
            try
            {
                await mealService.Delete(mealId, VendorId.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("recommended")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetRecommendedMeals([FromQuery] int take, [FromQuery] int skip, [FromServices] IRecommendationService recommendationService)
        {
            try
            {
                var meals = await recommendationService.GetRecommendedMeals(UserId.Value, skip, take);
                return Ok(meals);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{mealId:guid}/review")]
        public async Task<IActionResult> AddReview([FromRoute] Guid mealId, [FromBody] AddReviewDTO review)
        {
            try
            {
                await mealService.AddReview(mealId, review, UserId.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
