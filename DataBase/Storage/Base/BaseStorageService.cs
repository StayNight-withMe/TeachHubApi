using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Core.Interfaces.Utils;
using infrastructure.Storage.Implementation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Storage.Base
{
    public class BaseStorageService : IFileStorageService
    {
            private readonly IAmazonS3 _amazonS3;

            private readonly string _bucketName;

            protected ILogger _logger;

            public BaseStorageService(IAmazonS3 amazonS3,
                IOptions<BackblazeOptions> options
                )
            {
                _amazonS3 = amazonS3;
                _bucketName = options.Value.BucketName;
            }

            public async Task<bool> DeleteFileAsync(
                string fileName,
                CancellationToken ct = default)
            {

                try
                {
                    await _amazonS3.DeleteObjectAsync(_bucketName, fileName, ct);
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

                catch (TaskCanceledException ex)
                {
                    _logger.LogError(ex.Message);
                    return false;
                }


            }

            public virtual Task<Stream?> GetFileAsync(
                string fileName,
                CancellationToken ct = default
                )
            {
                throw new NotImplementedException();
            }


            public string GetPresignedUrl(
              string fileName,
              int min)
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

             protected async Task<string> UploadFileAsync(Stream fileStream,
                int fileid,
                string contentType,
                string directoryPath,
                string fileName = "lesson",
                CancellationToken ct = default
                )
            {

                var name = $"{Guid.NewGuid()}_{fileName}";

                var key = $"{directoryPath}/{fileid}/{DateTime.UtcNow:G}_{name}";

                await _amazonS3.PutObjectAsync(
                    new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        ContentType = contentType,
                        InputStream = fileStream,
                        AutoCloseStream = true
                    },
                    ct
                    );

                return key;

            }
        }
    }

