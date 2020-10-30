using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;

namespace Illusive.Illusive.Cdn.Interfaces {
    public interface IContentService {

        Task<string> UploadFileAsync(string fileName, FileStream fileStream);

    }
}