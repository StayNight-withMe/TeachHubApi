using Amazon.Runtime;
using Amazon.S3;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Interfaces.Utils;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.LessonFile.input;
using Core.Model.TargetDTO.LessonFile.output;
using infrastructure.Entitiеs;
using infrastructure.Extensions;
using infrastructure.Utils.PageService;
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

        private readonly IBaseRepository<LessonEntities> _lessonRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<LessonsStorageService> _logger;
        
        private readonly IFileStorageService _fileStorageService;
        
        public LessonsStorageService(
            IBaseRepository<LessonfilesEntities> lessonFileRepository,
            IBaseRepository<LessonEntities> lessonRepository,
            ILogger<LessonsStorageService>  logger,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork
            ) 
        {
        _fileStorageService = fileStorageService;
        _lessonFileRepository = lessonFileRepository;
        _lessonRepository = lessonRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        }
        public async Task<TResult> DeleteLessonUrlFile(
            int fileid, 
            int userid,
            CancellationToken ct = default
            )
        {
            var file = await _lessonFileRepository
                    .GetAllWithoutTracking()
                    .Where(c => c.id == fileid)
                    .FirstOrDefaultAsync();

            if (file == null)
            {
                return TResult.FailedOperation(errorCode.NotFound);
            }


            var lesson = await _lessonRepository
                .GetAllWithoutTracking()
                .Include(c => c.course)
                .Where(c => c.course.creatorid == userid &&
                 c.id == file.lessonid)
                .FirstOrDefaultAsync();

            if (lesson == null)
            {
                return TResult.FailedOperation(errorCode.NoRights);
            }
            

            try
            {
                await _fileStorageService.DeleteFileAsync(file.filekey, file.lessonid, ct);
                await _lessonFileRepository.DeleteById(file.id);
                await _unitOfWork.CommitAsync();
                return TResult.CompletedOperation();
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.CloudError);
            }
            catch(AmazonServiceException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.CloudError);
            }
            catch (AmazonClientException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.ClientError);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.TimeOutError);
            }
            catch(DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }


        }
        public async Task<TResult<PagedResponseDTO<LessonFileOutputDTO>>> GetLessonUrlFile(
            int lessonid, 
            PaginationDTO pagination,
            CancellationToken ct = default
            )
        {
            var lessonsFiles = await _lessonFileRepository
                .GetAllWithoutTracking()
                .Where(c => c.lessonid == lessonid)
                .GetWithPagination(pagination)
                .OrderBy(c => c.order)
                .ToListAsync();
            
            if (lessonsFiles.Count == 0)
            {
                return TResult<PagedResponseDTO<LessonFileOutputDTO>>.FailedOperation(errorCode.lessonNotFound);
            }

            
            var lessonOutPutDTOs = lessonsFiles
                .Select(c => new LessonFileOutputDTO
                {
                id = c.id,
                filename = c.filename,
                url = _fileStorageService.GetPresignedUrl(c.filekey, c.lessonid, 60, ct),
                order = c.order,
                })
                .ToList();


            return PageService.CreatePage(
                lessonOutPutDTOs,
                pagination,
                lessonOutPutDTOs.Count
                );

        }

        public async Task<TResult> UploadFile(
            Stream stream, 
            int userid, 
            MetaDataDTO metaData,
            string contentType,
            CancellationToken ct = default
            )
        {

           var lesson = await _lessonRepository.GetAllWithoutTracking()
                .Include(c => c.course)
                .Where(
                        c => c.id == metaData.lessonid && 
                        c.course.creatorid == userid)
                .FirstOrDefaultAsync();




            if (lesson == null)
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
                    filename = metaData.name,
                    lessonid = metaData.lessonid,
                    filekey = key,
                    cloudstore = metaData.cloudstore,
                    filetype = metaData.filetype,
                    order = metaData.order,
                }
                );

                await _unitOfWork.CommitAsync();

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
