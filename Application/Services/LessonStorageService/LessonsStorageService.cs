using Application.Abstractions.Repository.Base;
using Application.Abstractions.Repository.Custom;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using Application.Abstractions.Utils;
using Application.Utils.PageService;
using Core.Common.Exeptions;
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.LessonFile.input;
using Core.Models.TargetDTO.LessonFile.output;
using Core.Specification.LessonSpec;
using Core.Specification.LessonStorageSpec;
using Logger;
using Microsoft.Extensions.Logging;

namespace Application.Services.LessonStorageService
{
    public class LessonsStorageService : ILessonStorageService
    {
        private readonly ILessonFileRepository _lessonFileRepository;

        private readonly IBaseRepository<LessonEntity> _lessonRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<LessonsStorageService> _logger;

        private readonly ILessonFileService _lessonFileService;

        public LessonsStorageService(
            ILessonFileRepository lessonFileRepository,
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
            var fileData = await _lessonFileRepository
                .FirstOrDefaultAsync(new LessonFileDataSpec(fileid), ct);

            if (fileData == null)
            {
                return TResult.FailedOperation(errorCode.NotFound);
            }

            var isOwner = await _lessonRepository
                .FirstOrDefaultAsync(new LessonOwnerByFileSpec(fileData.LessonId, userid), ct);

            if (isOwner == null)
            {
                return TResult.FailedOperation(errorCode.NoRights);
            }

            try
            {
                await _lessonFileService.DeleteFileAsync(fileData.FileKey, ct);

                await _lessonFileRepository.DeleteById(ct, fileid);

                await _unitOfWork.CommitAsync(ct);

                return TResult.CompletedOperation();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.TimeOutError);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
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
            var spec = new LessonFilesByLessonSpec(lessonid);

            var lessonsFiles = await _lessonFileRepository.GetPagedLessonFilesAsync(spec, pagination, ct);

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

            var totalCount = await _lessonFileRepository.CountAsync(spec, ct);

            return PageService.CreatePage(
                lessonOutPutDTOs,
                pagination,
                totalCount 
            );
        }



        // не уверен в своем решении, но мне кажется что при ошибке в бд стоит удалять файл из облака
        //еще и токен есть
        public async Task<TResult> UploadFile(
        Stream stream,
        int userid,
        MetaDataLessonDTO metaData,
        string contentType,
        CancellationToken ct = default
    )
        {


            var valid = await _lessonFileRepository.AnyAsync(new LessonFileCreateSpec(
                metaData.lessonid.Value, 
                metaData.name, 
                metaData.order), ct);

            if(valid)
            {
                return TResult.FailedOperation(errorCode.LessonFileAlreadyExists);
            }



            var lessonId = await _lessonRepository
                .FirstOrDefaultAsync(new LessonOwnerSpec(metaData.lessonid.Value, userid), ct);

            //_logger.LogDebug($"userid: {userid}, lessonid: {metaData.lessonid.Value}");

            if (lessonId == 0)
            {
                return TResult.FailedOperation(errorCode.NoRights);
            }


            try
            {
                var key = await _lessonFileService.UploadFileAsync(
                    stream,
                    metaData.lessonid,
                    contentType,
                    ct: ct
                );

                await _lessonFileRepository.AddAsync(new LessonfileEntity
                {
                    filename = metaData.name,
                    lessonid = metaData.lessonid,
                    filekey = key,
                    cloudstore = metaData.cloudstore,
                    filetype = metaData.filetype,
                    order = metaData.order,
                }, ct);

                await _unitOfWork.CommitAsync(ct);

                return TResult.CompletedOperation();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.TimeOutError);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                await DeleteLessonUrlFile(userid, metaData.lessonid); 
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }
        }
    }
}
