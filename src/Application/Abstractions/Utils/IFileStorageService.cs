

namespace Application.Abstractions.Utils
{
    public interface IFileStorageService
    {
        
        Task<bool> DeleteFileAsync(
            string key,
            CancellationToken ct = default);
        Task<Stream?> GetFileAsync(
            string key,
            CancellationToken ct = default);
        string GetPresignedUrl(
            string fileName,
            int minutes);
       
    }
}

