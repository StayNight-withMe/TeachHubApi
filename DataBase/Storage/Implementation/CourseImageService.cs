using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Core.Interfaces.Utils;
using infrastructure.Storage.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Storage.Implementation
{
    public class CourseImageService : BaseStorageService, IFileService
    {
        public CourseImageService(
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
                  string fileName = "courseimg",
                  CancellationToken ct = default
                  )
        {
          return  await base.UploadFileAsync(fileStream,
                lessonid,
                contentType,
                "course-images",
                fileName,
                ct);
        }
    }
}

