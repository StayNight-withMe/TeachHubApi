using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using Application.Utils.PageService;
using AutoMapper;
using Core.Common.Exeptions;
using Logger;
using Microsoft.Extensions.Logging;
using Core.Specification.LessonSpec;
using Core.Models.TargetDTO.Lesson.input;
using Core.Models.TargetDTO.Lesson.output;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.Entitiеs;
using Core.Specification.CourseSpec;
using Application.Abstractions.Repository.Custom;

namespace Application.Services.LessonService
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;

        private readonly IBaseRepository<ChapterEntity> _chapterRepository;

        private readonly IBaseRepository<CourseEntity> _courseRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        public LessonService(ILogger<LessonService> logger, 
            IUnitOfWork unitOfWork, 
            IBaseRepository<ChapterEntity> chapterRepository,
            ILessonRepository lessonRepository,
            IBaseRepository<CourseEntity> courseRepository,
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
             var course = await _courseRepository
                .AnyAsync(new UserCourseIdSpecification(userid, lesson.courseid),ct);


            if (course == false)
            {
                return TResult.FailedOperation(errorCode.CoursesNotFoud);
            }


            bool valid = await _chapterRepository
                .AnyAsync(new ValidLessonForCreate(lesson.order, lesson.name));


            if(valid)
            {
                return TResult.FailedOperation(errorCode.LessonAlreadyExists);
            }


            await _lessonRepository.Create(_mapper.Map<LessonEntity>(lesson));
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
            var exists = await _lessonRepository.AnyAsync(new LessonByUserSpec(userid, lessonid)); 

             if(exists ==  false)
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
             var Outlist =  await _lessonRepository.GetLessonByChapterid(
                 chapterid, 
                 userSortingRequest, 
                 new GetLessonByChapter(chapterid), 
                 ct);

            return PageService.CreatePage(
                Outlist, 
                userSortingRequest,  
                await _lessonRepository.CountAsync(new GetLessonByChapter(chapterid),ct));

        }


        public async Task<TResult<PagedResponseDTO<LessonUserOutputDTO>>> GetUnVisibleLessonByChapterid(
            int userid, 
            int chapterid, 
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default)
        {
            var Outlist = await _lessonRepository.GetLessonUserByChapterid<LessonUserOutputDTO>(
                 chapterid,
                 userSortingRequest,
                 new GetLessonByChapter(chapterid, false, userid),
                 ct);

            return PageService.CreatePage(
                Outlist, 
                userSortingRequest, 
                await _lessonRepository.CountAsync(new GetLessonByChapter(chapterid, false), ct));
        }



        public async Task<TResult> SwitchVisible(
            int lessonid, 
            int userid,
            CancellationToken ct = default)
        {
            var lesson = await _lessonRepository
                .FirstOrDefaultAsync(new LessonByUserSpec(userid, lessonid, true) ,ct);

            if ( lesson == null )
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
                .FirstOrDefaultAsync(
                new LessonByUserSpec(userid, newlesson.id), 
                    ct);   
        
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
            catch(DbUpdateException ex) 
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
