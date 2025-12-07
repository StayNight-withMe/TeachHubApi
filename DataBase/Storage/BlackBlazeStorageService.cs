using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Core.Interfaces.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Storage
{
    public class BlackBlazeStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _amazonS3;
        
        private readonly string _bucketName;

        private readonly ILogger _logger;

        public BlackBlazeStorageService(IAmazonS3 amazonS3, 
            IOptions<BackblazeOptions> options,
            ILogger<BlackBlazeStorageService> logger
            )
        {
            _amazonS3 = amazonS3;
            _bucketName = options.Value.BucketName;
            _logger = logger;
        }

        public async Task<bool> DeleteFileAsync(
            string fileName, 
            int lessonid,
            CancellationToken ct = default)
        {
            
            try
            {
            await _amazonS3.DeleteObjectAsync(_bucketName, fileName, ct );
            return true;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
            catch (AmazonServiceException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
            catch (AmazonClientException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

           catch(TaskCanceledException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
 
            
        }

        public Task<Stream?> GetFileAsync(
            string fileName, 
            int lessonid, 
            CancellationToken ct = default
            )
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetPresignedUrlAsync(
            string fileName,
            int lessonid,
            TimeSpan expiry, 
            CancellationToken ct = default)
        {
            return 
                _amazonS3.GeneratePreSignedURL
                (
                _bucketName,
                fileName,
                DateTime.UtcNow.AddHours(2),
                additionalProperties: null
                );

        }

        public async Task<string> UploadFileAsync(Stream fileStream, 
            int lessonid, 
            string contentType, 
            string fileName = "lesson",
            CancellationToken ct = default
            )
        {

            var name = $"{Guid.NewGuid()}_{fileName}";

            var key = $"{lessonid}/{DateTime.UtcNow:G}_{name}";
            
            await _amazonS3.PutObjectAsync(
                new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    ContentType = contentType,
                    InputStream = fileStream,
                    AutoCloseStream = true
                }

                );

            return key;

        }
    }
}
