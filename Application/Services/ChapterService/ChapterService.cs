using Application.Abstractions.Repository.Base;
using Application.Abstractions.Repository.Custom;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using Application.Mapping.MapperDTO;
using Application.Utils.PageService;
using AutoMapper;
using Core.Common.Exeptions;
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Chapter.input;
using Core.Models.TargetDTO.Chapter.output;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Specification.CourseSpec;
using Core.Specifications.Chapters;
using Logger;
using Microsoft.Extensions.Logging;

namespace Application.Services.ChapterService
{
    public class ChapterService : IChapterService
    {

        private readonly IChapterRepository _chapterRepository;

        private readonly IBaseRepository<CourseEntity> _coursesRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        private readonly ILogger<ChapterService> _logger;

        public ChapterService(ILogger<ChapterService> logger,
            IUnitOfWork unitOfWork,
            IChapterRepository chapterRepository,
            IBaseRepository<CourseEntity> coursesRepository,
            IMapper mapper
            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _chapterRepository = chapterRepository;
            _coursesRepository = coursesRepository;
            _mapper = mapper;
        }


        public async Task<TResult> Create(
            CreateChapterDTO chapter, 
            int userid,
            CancellationToken ct = default)
        {
            CourseEntity? course = await _coursesRepository.
                FirstOrDefaultAsync(new CourseCreatorSpec(chapter.courseid, userid));

            if(course == null)
            {
                return TResult.FailedOperation(errorCode.CoursesNotFoud);
            }

            if (course.creatorid != userid)
            {
                return TResult<PagedResponseDTO<ChapterOutDTO>>.FailedOperation(errorCode.CoursesNotFoud, "нет прав");
            }

            await _chapterRepository.Create(_mapper.Map<ChapterEntity>(chapter));
            
            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError, "ошибка создания раздела");

            }
        }


        //public async Task<TResult<ChapterOutDTO>> GetChapterByID(int id)
        //{
        //    var chapter = await _chapterRepository.GetById(id);
        //    if (chapter == null)
        //    {
        //  return TResult<ChapterOutDTO>.FailedOperaion(errorCode.ChapterNotFound);
        //    }
        //    return TResult<ChapterOutDTO>.CompletedOperation(_mapper.Map<ChapterOutDTO>(chapter));
        //}

        public async Task<TResult<ChapterOutDTO>> UpdateChapter(
     ChapterUpdateDTO newchapter,
     int userid,
     CancellationToken ct = default)
        {

            var spec = new ChapterWithAccessSpec(newchapter.id, userid);

            var chapterEntity = await _chapterRepository.FirstOrDefaultAsync(spec, ct);

            if (chapterEntity == null)
            {
                return TResult<ChapterOutDTO>.FailedOperation(errorCode.ChapterNotFound);
            }

            await _chapterRepository.PartialUpdateAsync(chapterEntity, newchapter);

            try
            {
                await _unitOfWork.CommitAsync(ct);

                var resultDto = _mapper.Map<ChapterOutDTO>(chapterEntity);
                return TResult<ChapterOutDTO>.CompletedOperation(resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogDBError(ex);
                return TResult<ChapterOutDTO>.FailedOperation(errorCode.DatabaseError);
            }
        }


        public async Task<TResult<PagedResponseDTO<ChapterOutDTO>>> GetChaptersByCourseIdAndUserId(
     int courseid,
     int userid,
     SortingAndPaginationDTO userSortingRequest,
     CancellationToken ct = default)
        {
            var spec = new ChaptersByCourseSpec(courseid, userid);

            var chapterEntities = await _chapterRepository.GetPagedChaptersAsync(spec, userSortingRequest, ct);

            if (chapterEntities == null || !chapterEntities.Any())
            {
                return TResult<PagedResponseDTO<ChapterOutDTO>>.FailedOperation(
                    errorCode.CoursesNotFoud, "у вас отсутствует данный курс или разделы в нем");
            }

            var totalCount = await _chapterRepository.CountAsync(spec, ct);

            var dtoList = chapterEntities.Select(c => new ChapterOutDTO
            {
                id = c.id,
                name = c.name,
                order = (int)c.order,
            }).ToList();

            return PageService.CreatePage(dtoList, userSortingRequest, totalCount);
        }


        private  TResult<PagedResponseDTO<ChapterOutDTO>> GetChapter(
            List<ChapterEntity> chapterEntities, 
            SortingAndPaginationDTO userSortingRequest, 
            int count
            )
        {

            List<ChapterOutDTO> chapters = chapterEntities.Select(c => new ChapterOutDTO
            {
                id = c.id,
                name = c.name,
                order = (int)c.order,
            }).ToList();

            
            return PageService.CreatePage(chapters, userSortingRequest, count);
        }


        public async Task<TResult<PagedResponseDTO<ChapterOutDTO>>> GetChaptersByCourseId(
            int courseid,
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default)
        {
            var spec = new ChaptersByCoursePublicSpec(courseid);

            var chapterEntities = await _chapterRepository.GetPagedChaptersAsync(spec, userSortingRequest, ct);

            var totalCount = await _chapterRepository.CountAsync(spec, ct);

            var dtoList = chapterEntities.Select(c => new ChapterOutDTO
            {
                id = c.id,
                name = c.name,
                order = (int)c.order
            }).ToList();

            return PageService.CreatePage(dtoList, userSortingRequest, totalCount);
        }

        public async Task<TResult> DeleteChapter(
     int chapterid,
     int userid = default,
     CancellationToken ct = default)
        {
     
            var spec = new ChapterDeleteSpec(chapterid, userid == default ? null : userid);

            try
            {
                int deletedRows = await _chapterRepository.ExecuteDeleteBySpecAsync(spec, ct);

                if (deletedRows == 0)
                {
                    return TResult.FailedOperation(errorCode.ChapterNotFound);
                }

                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError, "Не удалось удалить раздел");
            }
            catch (Exception ex)
            {
                _logger.LogCriticalError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }
        }


    }

    }




