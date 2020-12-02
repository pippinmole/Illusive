using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Illusive {
    public interface IContentService {

        Task<string> UploadFileAsync(IFormFile formFile);
        Task<string> UploadFileAsync(string fileName, FileStream fileStream);

    }
}