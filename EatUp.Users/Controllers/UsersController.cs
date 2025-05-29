using EatUp.Users.DTO;
using EatUp.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Users.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUserService userService): ControllerBase
    {

        [HttpPost("signup")]
        public async Task<IActionResult> AddVendor([FromBody] AddUserDTO userDto)
        {
            try
            {
                await userService.AddUser(userDto);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInUserDTO signInUser)
        {
            try
            {
                return Ok(await userService.SignIn(signInUser));
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
                await userService.SignOut(refreshToken);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("me")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> UpdateVendor([FromBody] UpdateUserDTO userDTO, [FromHeader] Guid userId)
        {
            try
            {
                await userService.UpdateUser(userDTO, userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("me")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetUser([FromHeader] Guid userId)
        {
            try
            {
                UserDTO vendor = await userService.GetUserById(userId);
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
                UserTokens token = await userService.RefreshToken(refreshToken);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("me")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Delete([FromHeader] Guid userId)
        {
            try
            {
                await userService.Delete(userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("favorites")]
        public async Task<IActionResult> AddToFavorites([FromBody] Guid mealId, [FromHeader] Guid userId)
        {
            try
            {
                await userService.AddToFavorite(mealId, userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("favorites")]
        public async Task<IActionResult> UnFavorite([FromBody] Guid mealId, [FromHeader] Guid userId)
        {
            try
            {
                await userService.UnFavorite(mealId, userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavorites([FromHeader] Guid userId)
        {
            try
            {
                return Ok(await userService.GetFavorites(userId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
