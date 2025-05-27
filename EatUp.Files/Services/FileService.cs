using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EatUp.Files
{
    public class FileService(string connectionString) : IFileService
    {
        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            string blobName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                });
            }

            return blobClient.Uri.AbsolutePath;
        }

        public async Task DeleteBlobAsync(string blobName, string containerName)
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public static (string, string) GetBlobInfoFromUrl(string path)
        {
            string absolutePath = path.Replace("devstoreaccount1/", "");

            var segments = absolutePath.TrimStart('/').Split('/', 2);

            if (segments.Length < 2)
            {
                throw new ArgumentException("URL does not contain a valid blob path.");
            }

            string containerName = Uri.UnescapeDataString(segments[0]);
            string blobName = Uri.UnescapeDataString(segments[1]);
            return (containerName, blobName);
        }
    }
}
