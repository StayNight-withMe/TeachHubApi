using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

