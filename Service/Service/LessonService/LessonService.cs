using AutoMapper;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Lesson.input;
using Core.Model.TargetDTO.Lesson.output;
using infrastructure.Entitiеs;
using infrastructure.Extensions;
using infrastructure.Utils.PageService;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Applcation.Service.LessonService
{
    public class LessonService : ILessonService
    {
        private readonly IBaseRepository<LessonEntities> _lessonRepository;

        private readonly IBaseRepository<ChapterEntity> _chapterRepository;

        private readonly IBaseRepository<CourseEntities> _courseRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        public LessonService(ILogger<LessonService> logger, 
            IUnitOfWork unitOfWork, 
            IBaseRepository<ChapterEntity> chapterRepository, 
            IBaseRepository<LessonEntities> lessonRepository,
            IBaseRepository<CourseEntities> courseRepository,
            IMapper mapper
            ) 
        { 
           _logger = logger;
            _unitOfWork = unitOfWork;
            _courseRepository = courseRepository;
            _chapterRepository = chapterRepository;
            _lessonRepository = lessonRepository;
            _mapper = mapper;
        }

        public async Task<TResult> Create(
            createLessonDTO lesson, 
            int userid,
            CancellationToken ct = default)
        {
             var course = await _courseRepository.GetAllWithoutTracking()
                .Where(c => c.creatorid == userid && c.id == lesson.courseid)
                .FirstOrDefaultAsync(ct);


            if ( course == null )
            {
                return TResult.FailedOperation(errorCode.CoursesNotFoud);
            }


            bool valid = await _chapterRepository.GetAllWithoutTracking()
            .Where(c => c.order == lesson.order &&
                   c.name == lesson.name)
            .AnyAsync(ct);


            if(valid)
            {
                return TResult.FailedOperation(errorCode.LessonAlreadyExists);
            }


            await _lessonRepository.Create(_mapper.Map<LessonEntities>(lesson));
            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
           

        }

        public async Task<TResult> DeleteLessonForAdmin(
            int lessonid,
            CancellationToken ct = default)
        {
            await _lessonRepository.DeleteById(ct, lessonid);
            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }

        }

        public async Task<TResult> DeleteLessonForUser(
            int lessonid, 
            int userid,
            CancellationToken ct = default)
        {
            var course = await _lessonRepository
               .GetAllWithoutTracking()
               .Include(c => c.course)
               .Where(c => c.course.creatorid == userid)
               .FirstOrDefaultAsync(); 

             if( course == null )
            {
                return TResult.FailedOperation(errorCode.CoursesNotFoud);
            }

             await _lessonRepository.DeleteById(ct, lessonid);

            try
            {
                await _unitOfWork.CommitAsync();
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex) 
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

        public async Task<TResult<PagedResponseDTO<lessonOutputDTO>>> GetLessonByChapterid(
            int chapterid, 
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default
            )
        {
             var qwery =  _lessonRepository.GetAllWithoutTracking()
                .Where(c => c.chapterid == chapterid  && 
                       c.isvisible == true);
            var lessons = await qwery
                .GetWithPaginationAndSorting(userSortingRequest, "isvisible", "chapterid", "id")
                .ToListAsync(ct);


            List<lessonOutputDTO> Outlist = lessons.Select(c => new lessonOutputDTO
            {
                name = c.name,
                id = c.id,
                order = (int)c.order
            }
            ).ToList();

             
            return PageService.CreatePage(Outlist, userSortingRequest,  await qwery.CountAsync(ct));

        }


        public async Task<TResult<PagedResponseDTO<LessonUserOutputDTO>>> GetUnVisibleLessonByChapterid(
            int userid, 
            int chapterid, 
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default)
        {
            var qwery = _lessonRepository.GetAllWithoutTracking()
                .Include(c => c.course)
                .Where(c => c.chapterid == chapterid && 
                       c.course.creatorid == userid);
            var lessons = await qwery.GetWithPaginationAndSorting(userSortingRequest, "isvisible", "chapterid", "id").ToListAsync(ct);

            List<LessonUserOutputDTO> Outlist = lessons.Select(c => new LessonUserOutputDTO
            {
                name = c.name,
                id = c.id,
                order = (int)c.order,
                isvisible = c.isvisible
            }
            ).ToList();

            return PageService.CreatePage(Outlist, userSortingRequest, await qwery.CountAsync(ct));
        }



        public async Task<TResult> SwitchVisible(
            int lessonid, 
            int userid,
            CancellationToken ct = default)
        {
            var lesson = await _lessonRepository.GetAll()
                .Include(c => c.course)
                .Where(c => c.course.creatorid == userid && c.id == lessonid)
                .FirstOrDefaultAsync(ct);

            if( lesson == null )
            {
                return TResult.FailedOperation(errorCode.lessonNotFound);
            }

            lesson.isvisible = !lesson.isvisible;

            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex)
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

        public async Task<TResult<lessonOutputDTO>> UpdateLesson(
            LessonUpdateDTO newlesson, 
            int userid,
            CancellationToken ct = default
            )
        {
            var lesson = await _lessonRepository
                .GetAllWithoutTracking()
                .Include(c => c.course)
                .Where(c => c.id == newlesson.id && c.course.creatorid == userid)
                .FirstOrDefaultAsync(ct);   
        
            if( lesson == null )
            {
              return TResult<lessonOutputDTO>.FailedOperation(errorCode.lessonNotFound);
            }

            await _lessonRepository.PartialUpdateAsync(lesson, newlesson);
            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult<lessonOutputDTO>.CompletedOperation(_mapper.Map<lessonOutputDTO>(lesson));
            }
            catch( DbUpdateException ex) 
            {
                _logger.LogDBError(ex);
                return TResult<lessonOutputDTO>.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return TResult<lessonOutputDTO>.FailedOperation(errorCode.UnknownError);
            }

        }
    }
}
