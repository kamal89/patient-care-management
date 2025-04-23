using PatientCareManagement.Core.Interfaces;
using System.Collections.Concurrent;

namespace PatientCareManagement.Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly ConcurrentDictionary<string, byte[]> _blobs = new ConcurrentDictionary<string, byte[]>();

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            // Generate a unique blob ID
            var blobId = Guid.NewGuid().ToString();
            
            // Read the file stream into a byte array
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                _blobs[blobId] = bytes;
            }
            
            return blobId;
        }

        public Task<Stream?> DownloadFileAsync(string blobId)
        {
            if (_blobs.TryGetValue(blobId, out var blobData))
            {
                return Task.FromResult<Stream?>(new MemoryStream(blobData));
            }
            
            return Task.FromResult<Stream?>(null);
        }

        public Task DeleteFileAsync(string blobId)
        {
            _blobs.TryRemove(blobId, out _);
            return Task.CompletedTask;
        }
    }
}