using Azure.Storage.Blobs;
using Diplom_project_2024.Data;

namespace Diplom_project_2024.Functions
{
    public static class BlobContainerFunctions
    {
        public static async Task<string> UploadImage(BlobContainerClient container, IFormFile image)
        {
            var blob = container.GetBlobClient($"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}");
            await blob.UploadAsync(image.OpenReadStream());
            return blob.Uri.AbsoluteUri;
        }
        public static async void DeleteImage(BlobContainerClient container, string ImagePath)
        {
            var blob = container.GetBlobClient(Path.GetFileName(ImagePath));
            await blob.DeleteIfExistsAsync();
        }
    }
}
