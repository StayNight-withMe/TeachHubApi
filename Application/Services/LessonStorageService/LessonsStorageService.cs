using Amazon.Runtime;
using Amazon.S3;
using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using Application.Abstractions.Utils;
using Application.Utils.PageService;
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.LessonFile.input;
using Core.Models.TargetDTO.LessonFile.output;
using Core.Specification.Extensions.Other;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.LessonStorageService
{
    public class LessonsStorageService : ILessonStorageService
    {
        private readonly IBaseRepository<LessonfilesEntities> _lessonFileRepository;

        private readonly IBaseRepository<LessonEntity> _lessonRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<LessonsStorageService> _logger;

        private readonly ILessonFileService _lessonFileService;

        public LessonsStorageService(
            IBaseRepository<LessonfilesEntities> lessonFileRepository,
            IBaseRepository<LessonEntity> lessonRepository,
            ILogger<LessonsStorageService>  logger,
            IFileStorageService fileStorageService,
            ILessonFileService fileService,
            IUnitOfWork unitOfWork
            ) 
        {
        _lessonFileRepository = lessonFileRepository;
        _lessonRepository = lessonRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _lessonFileService = fileService;
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
                    .FirstOrDefaultAsync(ct);

            if (file == null)
            {
                return TResult.FailedOperation(errorCode.NotFound);
            }


            var lesson = await _lessonRepository
                .GetAllWithoutTracking()
                .Include(c => c.course)
                .Where(c => c.course.creatorid == userid &&
                 c.id == file.lessonid)
                .FirstOrDefaultAsync(ct);

            if (lesson == null)
            {
                return TResult.FailedOperation(errorCode.NoRights);
            }

            await _lessonFileRepository.DeleteById(ct, file.id);
            try
            {
                await _lessonFileService.DeleteFileAsync(file.filekey, ct);
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
                .ToListAsync(ct);
            
            if (lessonsFiles.Count == 0)
            {
                return TResult<PagedResponseDTO<LessonFileOutputDTO>>.FailedOperation(errorCode.lessonNotFound);
            }

            
            var lessonOutPutDTOs = lessonsFiles
                .Select(c => new LessonFileOutputDTO
                {
                id = c.id,
                filename = c.filename,
                url = _lessonFileService.GetPresignedUrl(c.filekey, 60),
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
            MetaDataLessonDTO metaData,
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
                var key = await _lessonFileService.UploadFileAsync(
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

                await _unitOfWork.CommitAsync(ct);

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
