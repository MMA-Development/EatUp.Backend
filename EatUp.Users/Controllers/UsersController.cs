using EatUp.Users.DTO;
using EatUp.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Users.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUserService userService): EatUpController
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
        public async Task<IActionResult> UpdateVendor([FromBody] UpdateUserDTO userDTO)
        {
            try
            {
                await userService.UpdateUser(userDTO, UserId.Value);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("me")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                UserDTO vendor = await userService.GetUserById(UserId.Value);
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
        public async Task<IActionResult> Delete()
        {
            try
            {
                await userService.Delete(UserId.Value);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("favorites")]
        public async Task<IActionResult> AddToFavorites([FromBody] Guid mealId)
        {
            try
            {
                await userService.AddToFavorite(mealId, UserId.Value);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("favorites")]
        public async Task<IActionResult> UnFavorite([FromBody] Guid mealId)
        {
            try
            {
                await userService.UnFavorite(mealId, UserId.Value);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavorites()
        {
            try
            {
                return Ok(await userService.GetFavorites(UserId.Value));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
