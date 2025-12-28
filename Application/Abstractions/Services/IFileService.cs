using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applcation.Abstractions.Service
{
    public interface IFileService : IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream,
            int fileid,
            string contentType,
            string fileName = "file",
            CancellationToken ct = default);
    }
}
