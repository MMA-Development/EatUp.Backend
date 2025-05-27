using Microsoft.AspNetCore.Mvc;

namespace EatUp.Files.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController: Controller
    {
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok(new {Filename = file.FileName});
        }
    }
}
