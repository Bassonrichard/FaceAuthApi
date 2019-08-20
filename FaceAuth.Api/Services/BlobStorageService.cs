using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FaceAuth.Api.Services
{
    public interface IBlobStorageService
    {
        Task<string> WriteImageToBlob(byte[] Image, ILogger _logger);
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();
        public  async Task<string> WriteImageToBlob(byte[] Image, ILogger _logger)
        {
            CloudStorageAccount cloudStorageAccount;
            CloudBlockBlob cloudBlockBlob;
            CloudBlobClient cloudBlobClient;
            CloudBlobContainer cloudBlobContainer;

            try
            {
                cloudStorageAccount = CloudStorageAccount.Parse(Settings.StorageURL);

                cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

                cloudBlobContainer = cloudBlobClient.GetContainerReference("user-images");

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }


                var ImageId = Guid.NewGuid().ToString()+".jpg";
                cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(ImageId);

                await cloudBlockBlob.UploadFromByteArrayAsync(Image, 0, Image.Length);

                cloudBlockBlob.Properties.ContentType = "image/jpg";
                await cloudBlockBlob.SetPropertiesAsync();

                return cloudBlockBlob.Uri.AbsoluteUri.ToString();

            }
            catch (Exception ex)
            {
                _logger.LogError("Error Writing to blob storage: ", ex);
                throw;
            }

        }
    }
}
