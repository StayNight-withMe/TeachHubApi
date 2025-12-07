using Amazon.Runtime;
using Amazon.S3;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.Utils;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.LessonFile.input;
using Core.Model.TargetDTO.LessonFile.output;
using infrastructure.Entitiеs;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Applcation.Service.LessonStorageService
{
    public class LessonsStorageService : ILessonStorageService
    {
        private readonly IBaseRepository<LessonfilesEntities> _lessonFileRepository;
        
        private readonly ILogger<LessonsStorageService> _logger;
        
        private readonly IFileStorageService _fileStorageService;
        
        public LessonsStorageService(
            IBaseRepository<LessonfilesEntities> lessonRepository,
            ILogger<LessonsStorageService>  logger,
            IFileStorageService fileStorageService
            ) 
        {
        _fileStorageService = fileStorageService;
        _lessonFileRepository = lessonRepository;
        _logger = logger;
        }
        public Task DeleteLessonUrlFile(string fileid, int lessonid, int userid)
        {
            throw new NotImplementedException();
        }

        public Task<TResult<PagedResponseDTO<LessonFileOutputDTO>>> GetLessonUrlFile(int lessonid)
        {
            throw new NotImplementedException();
        }

        public async Task<TResult> UploadFile(Stream stream, 
            int userid, 
            MetaDataDTO metaData,
            string contentType,
            CancellationToken ct = default
            )
        {

            var user = await _lessonFileRepository.
                GetAllWithoutTracking()
                .Include(c => c.lesson)
                .Include(c => c.lesson.course)
                .Where(c => c.lesson.course.creatorid == userid)
                .FirstOrDefaultAsync();

            if(user == null)
            {
                return TResult.FailedOperation(errorCode.NoRights);
            }

            try
            {
                var key = await _fileStorageService.UploadFileAsync(
                    stream, 
                    metaData.lessonid, 
                    contentType,
                    ct : ct
                    );
                await _lessonFileRepository.Create(new LessonfilesEntities 
                { 
                 lessonid = metaData.lessonid,
                 filekey = key,
                 cloudstore = metaData.cloudstore,
                 filetype = metaData.filetype,
                 order = metaData.order,
                 fileurl = metaData.fileurl,
                } 
                );
                return TResult.CompletedOperation();
            }
            catch(AmazonS3Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.CloudError);
            }
            catch(AmazonServiceException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.CloudError);
            }
            catch(AmazonClientException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.ClientError);
            }
            catch(TaskCanceledException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.TimeOutError);
            }
        }
    }
}
