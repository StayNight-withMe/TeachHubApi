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
using infrastructure.Repository.Base;
using infrastructure.Utils.PageService;
using infrastructure.Utils.SortBuilder;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public LessonService(ILogger logger, 
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

        public async Task<TResult> Create(createLessonDTO lesson, int userid)
        {
             var course = await _courseRepository.GetAllWithoutTracking()
                .Where(c => c.creatorid == userid && c.id == lesson.courseid)
                .FirstOrDefaultAsync();

             if( course == null )
            {
                return TResult.FailedOperation(errorCode.CoursesNotFoud);
            }

            await _lessonRepository.Create(_mapper.Map<LessonEntities>(lesson));
            try
            {
                await _unitOfWork.CommitAsync();
                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
           

        }

        public async Task<TResult> DeleteLesson(int lessonid, int userid)
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

             await _lessonRepository.DeleteById(lessonid);

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
        }

        public async Task<TResult<PagedResponseDTO<lessonOutputDTO>>> GetLessonByChapterid(int chapterid, UserSortingRequest userSortingRequest)
        {
             var qwery =  _lessonRepository.GetAllWithoutTracking().Where(c => c.chapterid == chapterid  && c.isVisible == true);
            var lessons = await qwery.GetWithPagination(userSortingRequest).ToListAsync();

            //if ( lessons.Count == 0)
            //{
            //    return TResult<PagedResponseDTO<lessonOutputDTO>>.FailedOperation(errorCode.CoursesNotFoud);
            //}

            List<lessonOutputDTO> Outlist = lessons.Select(c => new lessonOutputDTO
            {
                name = c.name,
                id = c.id,
                order = c.order
            }
            ).ToList();


            SortBuilder<lessonOutputDTO> sortBuilder = new(Outlist);
             
            return PageService.CreatePage(sortBuilder, userSortingRequest,  await qwery.CountAsync());

        }


        public async Task<TResult<PagedResponseDTO<lessonOutputDTO>>> GetUnVisibleLessonByChapterid(int userid, int chapterid, UserSortingRequest userSortingRequest)
        {
            var qwery = _lessonRepository.GetAllWithoutTracking()
                .Include(c => c.course)
                .Where(c => c.chapterid == chapterid && c.isVisible == false && c.course.creatorid == userid);
            var lessons = await qwery.GetWithPagination(userSortingRequest).ToListAsync();

            //if (lessons.Count == 0)
            //{
            //    return TResult<PagedResponseDTO<lessonOutputDTO>>.FailedOperation(errorCode.lessonNotFound);
            //}

            List<lessonOutputDTO> Outlist = lessons.Select(c => new lessonOutputDTO
            {
                name = c.name,
                id = c.id,
                order = c.order
            }
            ).ToList();


            SortBuilder<lessonOutputDTO> sortBuilder = new(Outlist, "id");

            return PageService.CreatePage(sortBuilder, userSortingRequest, await qwery.CountAsync());

        }



        public async Task<TResult> SwitchVisible(int lessonid, int userid)
        {
            var lesson = await _lessonRepository.GetAllWithoutTracking()
                .Include(c => c.course)
                .Where(c => c.course.creatorid == userid && c.id == lessonid)
                .FirstOrDefaultAsync();

            if( lesson == null )
            {
                return TResult.FailedOperation(errorCode.lessonNotFound);
            }

            lesson.isVisible = !lesson.isVisible;

            await _lessonRepository.Update(lesson);

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
            
        }

        public async Task<TResult<lessonOutputDTO>> UpdateLesson(LessonUpdateDTO newlesson, int userid)
        {
            var lesson = await _lessonRepository.GetAllWithoutTracking().Include(c => c.course).Where(c => c.id == newlesson.id && c.course.creatorid == userid).FirstOrDefaultAsync();   
        
            if( lesson == null )
            {
              return TResult<lessonOutputDTO>.FailedOperation(errorCode.lessonNotFound);
            }

            await _lessonRepository.PartialUpdateAsync(lesson, newlesson);
            try
            {
                await _unitOfWork.CommitAsync();
                return TResult<lessonOutputDTO>.CompletedOperation(_mapper.Map<lessonOutputDTO>(lesson));
            }
            catch( DbUpdateException ex) 
            {
                _logger.LogDBError(ex);
                return TResult<lessonOutputDTO>.FailedOperation(errorCode.DatabaseError);
            }

        }
    }
}
