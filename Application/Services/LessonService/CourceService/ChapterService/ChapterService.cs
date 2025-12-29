using Applcation.Abstractions.Repository.Base;
using Applcation.Abstractions.UoW;
using AutoMapper;
using Core.Interfaces.Service;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Chapter.input;
using Core.Model.TargetDTO.Chapter.output;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using infrastructure.DataBase.Entitiеs;
using infrastructure.Extensions;
using infrastructure.Utils.Mapping.MapperDTO;
using infrastructure.Utils.PageService;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Applcation.Services.LessonService.CourceService.ChapterService
{
    public class ChapterService : IChapterService
    {

        private readonly IBaseRepository<ChapterEntity> _chapterRepository;

        private readonly IBaseRepository<CourseEntity> _coursesRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        private readonly ILogger<ChapterService> _logger;

        public ChapterService(ILogger<ChapterService> logger,
            IUnitOfWork unitOfWork,
            IBaseRepository<ChapterEntity> chapterRepository,
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
            CourseEntity? course = await _coursesRepository.GetAll().AsNoTracking().
                Where(c => c.creatorid == userid && c.id == chapter.courseid).
                FirstOrDefaultAsync();

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
        //
        //  return TResult<ChapterOutDTO>.FailedOperaion(errorCode.ChapterNotFound);
        //    }
        //    return TResult<ChapterOutDTO>.CompletedOperation(_mapper.Map<ChapterOutDTO>(chapter));
        //}

        public async Task<TResult<ChapterOutDTO>> UpdateChapter(
            ChapterUpdateDTO newchapter,
            int userid,
            CancellationToken ct = default
            )
        {

            var courses = _coursesRepository.GetAllWithoutTracking().Where(c => c.creatorid == userid);

            var chapters = _chapterRepository.GetAllWithoutTracking().Where(c => c.id == newchapter.id); //пустой может быть при неккоректном фронте
   
            var joined = courses
               .Join(chapters,
               c => c.id,
               c => c.courseid,
               (courses, chapter) => new 
               {
                   courseid = courses.id,

                   chapter
               }
               );

            var chapterInfo = await joined.FirstOrDefaultAsync(ct);

            if (chapterInfo == null || chapterInfo.chapter == null)
                return TResult<ChapterOutDTO>.FailedOperation(errorCode.ChapterNotFound);

            var source = new ChapterMappingSource()
            {
                courseid = chapterInfo.courseid,
                Chapter = chapterInfo.chapter, 
            };


            var partUpdate = _mapper.Map<ChapterEntity>(source);

            await _chapterRepository.PartialUpdateAsync(chapterInfo.chapter, newchapter);
          
            try
            {

                await _unitOfWork.CommitAsync(ct);
                return TResult<ChapterOutDTO>.CompletedOperation(_mapper.Map<ChapterOutDTO>(chapterInfo.chapter));
            }
            catch(Exception ex)
            {
                _logger.LogDBError(ex);
                return TResult<ChapterOutDTO>.FailedOperation(errorCode.DatabaseError);
                
            }
           
        }


        public async Task<TResult<PagedResponseDTO<ChapterOutDTO>>> GetChaptersByCourseIdAndUserId(
            int courseid, 
            int userid, 
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default
            )
        {
            var chapter = await _chapterRepository.GetAllWithoutTracking().GetWithPaginationAndSorting(userSortingRequest, "id", "courseid")
                .Include(c => c.course)
                .Where(c => c.course.creatorid == userid && c.course.id == courseid)
                .ToListAsync(ct);

            if(chapter == null || chapter.Count() == 0)
            {
                return TResult<PagedResponseDTO<ChapterOutDTO>>.FailedOperation(errorCode.CoursesNotFoud, "у вас отсутствует данный курс");
            }

                return GetChapter(chapter, userSortingRequest, await _chapterRepository.GetAllWithoutTracking().Include(c => c.course).Where(c => c.course.id == courseid).CountAsync());
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
            var chapter = await _chapterRepository
                .GetAllWithoutTracking()
                .GetWithPaginationAndSorting(userSortingRequest, "id", "courseid")
                .Include(c => c.course)
                .Where(c => c.course.id == courseid)
                .ToListAsync(ct);

            return GetChapter(chapter, userSortingRequest, await _chapterRepository.GetAllWithoutTracking().Include(c => c.course).Where(c => c.course.id == courseid).CountAsync());

        }

        public async Task<TResult> DeleteChapter(
            int chapterid,
            int userid = default,
            CancellationToken ct = default)
        {

            try
            {
                if(userid == default)
                {
                   await _chapterRepository
                        .GetAll()
                        .Where(c => c.id == chapterid)
                        .ExecuteDeleteAsync();
                }
                else
                {
                    await _chapterRepository
                             .GetAll()
                             .Include(c => c.course)
                             .Where(c => c.id == chapterid &&
                                    c.course.creatorid == userid)
                             .ExecuteDeleteAsync();
                }
         
                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.ChapterNotFound);
            }      
        }


    }

    }




