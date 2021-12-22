using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive {
    public class ContentService : IContentService {
        private readonly ILogger<ContentService> _logger;

        public BlobContainerClient Client { get; }
        public string ConnectionString { get; }

        private readonly string _containerName = "container-1";
        
        public ContentService(ILogger<ContentService> logger, IConfiguration configuration) {
            _logger = logger;
            ConnectionString = configuration.GetConnectionString("CdnConnectionString");
            
            // Get a reference to a container named "container-1" and then create it
            Client = new BlobContainerClient(ConnectionString, "container-1");

            // // Get a reference to a blob named "sample-file" in a container named "sample-container"
            // var blob = container.GetBlobClient(blobName);
            //
            // // Upload local file
            // blob.Upload(filePath);
        }

        /// <summary>
        /// Converts the IFormFile into a FileStream, and uploads it to Azure
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="formFile"></param>
        /// <returns>The URL to the image</returns>
        public async Task<string> UploadFileAsync(IFormFile formFile) {
            await using var stream = File.Create(Path.GetTempFileName(), (int) formFile.Length);
            await formFile.CopyToAsync(stream);

            return await UploadFileAsync(Path.GetFileName(formFile.FileName), stream);
        }

        public async Task<string> UploadFileAsync(string fileName, FileStream fileStream) {
            fileStream.Position = 0;
            var id = Guid.NewGuid();
            await Client.UploadBlobAsync($"{id}{fileName}", fileStream);

            return $"https://illusivecdn.blob.core.windows.net/{_containerName}/{id}{fileName}";
        }
    }
}