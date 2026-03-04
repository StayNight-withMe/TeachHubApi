using Amazon.S3;
using Application.Abstractions.Utils;
using Core.Models.Options;
using infrastructure.Storage.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace infrastructure.Storage.Implementation
{
    public class LessonFileStorageService : BaseStorageService, ILessonFileService
    {
        public LessonFileStorageService(
            IAmazonS3 amazonS3,
            IOptions<BackblazeOptions> options,
            ILogger<CourseImageService> logger)
            : base(amazonS3, options) 
        {
        _logger = logger;
        }

        public async Task<string> UploadFileAsync(
            Stream fileStream,
                  int lessonid,
                  string contentType,
                  string fileName = "lesson",
                  CancellationToken ct = default
                  )
        {
            return await base.UploadFileAsync(fileStream,
                  lessonid,
                  contentType,
                  "lessons",
                  fileName,
                  ct);
        }
    }
}
