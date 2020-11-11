using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;

namespace Illusive {
    public interface IContentService {

        Task<string> UploadFileAsync(string fileName, FileStream fileStream);

    }
}