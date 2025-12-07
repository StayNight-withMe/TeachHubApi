using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Utils
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream,
            int lessonid,
            string contentType,
            string fileName = "lesson",
            CancellationToken ct = default);
        Task<bool> DeleteFileAsync(
            string fileName,
            int lessonid,
            CancellationToken ct = default);
        Task<Stream?> GetFileAsync(
            string fileName,
            int lessonid,
            CancellationToken ct = default);
        Task<string> GetPresignedUrlAsync(
            string fileName,
            int lessonid,
            TimeSpan expiry, 
            CancellationToken ct = default);
    }
}

