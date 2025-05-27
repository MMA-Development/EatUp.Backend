using Microsoft.AspNetCore.Mvc;

namespace EatUp.Files.Controllers
{
    [ApiController]
    [Route("[controller]/{containerName}")]
    public class FilesController(IFileService fileService) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string containerName)
        {
            try
            {
                var result = await fileService.UploadFileAsync(file, containerName);
                return Ok(new { Fileurl = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "File upload failed", Error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string blobUrl)
        {
            try
            {
                var (containerName, blobName) = FileService.GetBlobInfoFromUrl(blobUrl);
                await fileService.DeleteBlobAsync(blobName, containerName);
                return Ok(new { Message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "File deletion failed", Error = ex.Message });
            }
        }
    }
}