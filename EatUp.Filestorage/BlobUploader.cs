using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace EatUp.Filestorage
{
    public class FileHandler(string connectionString, string containerName)
    {
        public async Task<Uri> UploadFileAsync(IFormFile file)
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync();

            string blobName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: false);
            }

            return blobClient.Uri;
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public static string GetBlobNameFromUrl(string blobUrl)
        {
            var uri = new Uri(blobUrl);

            string absolutePath = uri.AbsolutePath;

            var segments = absolutePath.TrimStart('/').Split('/', 2);

            if (segments.Length < 2)
            {
                throw new ArgumentException("URL does not contain a valid blob path.");
            }

            string blobName = Uri.UnescapeDataString(segments[1]); // decode URL
            return blobName;
        }
    }
}
