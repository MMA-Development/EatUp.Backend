using EatUp.Users.Models;
using EatUp.Users.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Users.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUserService mealService): ControllerBase
    {

        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            try
            {
                mealService.AddUser(user);
                return Ok();
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
