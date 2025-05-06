using EatUp.Meals.DTO;
using EatUp.Meals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Meals.Controllers
{
    [ApiController]
    [Route("meals/categories")]
    [Authorize]
    public class CategoriesController(ICategoryService categoryService): Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                // Assuming you have a service to get categories
                var categories = await categoryService.GetPage(skip, take);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "Vendor")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDTO addCategoryDTO)
        {
            try
            {
                var categoryId = await categoryService.Create(addCategoryDTO);
                return CreatedAtAction(nameof(GetCategories), new { id = categoryId }, null);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
