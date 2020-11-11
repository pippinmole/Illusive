using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive {
    public class ContentService : IContentService {
        private readonly ILogger<ContentService> _logger;

        public BlobContainerClient Client { get; }
        public string ConnectionString { get; }

        private readonly string _containerName = "container-1";
        
        public ContentService(ILogger<ContentService> logger, IConfiguration configuration) {
            this._logger = logger;
            this.ConnectionString = configuration.GetConnectionString("CdnConnectionString");
            
            // Get a reference to a container named "container-1" and then create it
            this.Client = new BlobContainerClient(this.ConnectionString, "container-1");

            // // Get a reference to a blob named "sample-file" in a container named "sample-container"
            // var blob = container.GetBlobClient(blobName);
            //
            // // Upload local file
            // blob.Upload(filePath);
        }
        
        public async Task<string> UploadFileAsync(string fileName, FileStream fileStream) {
            fileStream.Position = 0;
            var id = Guid.NewGuid();
            await this.Client.UploadBlobAsync($"{id}{fileName}", fileStream);

            return $"https://illusivecdn.blob.core.windows.net/{this._containerName}/{id}{fileName}";
        }
    }
}