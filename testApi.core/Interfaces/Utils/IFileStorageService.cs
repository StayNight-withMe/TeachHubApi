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
            string key,
            int lessonid,
            CancellationToken ct = default);
        Task<Stream?> GetFileAsync(
            string key,
            int lessonid,
            CancellationToken ct = default);
        string GetPresignedUrl(
            string fileName,
            int lessonid,
            int minutes, 
            CancellationToken ct = default);
       
    }
}

