using PatientCareManagement.Core.Models;

namespace PatientCareManagement.Core.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        Task<Stream?> DownloadFileAsync(string blobId);
        Task DeleteFileAsync(string blobId);
    }
}
