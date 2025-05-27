
namespace EatUp.Files
{
    public interface IFileService
    {
        Task DeleteBlobAsync(string blobName, string containerName);
        Task<string> UploadFileAsync(IFormFile file, string containerName);
    }
}