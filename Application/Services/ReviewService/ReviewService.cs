using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using Application.Utils.PageService;
using AutoMapper;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Review.input;
using Core.Model.TargetDTO.Review.output;
using infrastructure.DataBase.Entitiеs;
using infrastructure.Extensions;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IBaseRepository<ReviewEntities> _reviewRepository;

        private readonly IBaseRepository<CourseEntity> _courseRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ReviewService> _logger;

        private readonly IMapper _mapper;

        public ReviewService(
        IMapper mapper,
        IBaseRepository<ReviewEntities> reviewRepository,
        IBaseRepository<CourseEntity> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReviewService> logger
            ) 
        {
        _reviewRepository = reviewRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        }

        public async Task<TResult<ReviewOutputDTO>> UpdateReview(
            ReviewChangedDTO changedDTO, 
            int userid, 
            CancellationToken ct = default)
        {
            var review = await _reviewRepository
                .GetAllWithoutTracking()
                .Where(c => c.userid == userid &&
                       c.id == changedDTO.reviewid)
                .FirstOrDefaultAsync(ct);

            if (review == null) 
            {
                return TResult<ReviewOutputDTO>.FailedOperation(errorCode.ReviewNotFound);
            }

            review.lastchangedat = DateTime.UtcNow;

            await _reviewRepository.PartialUpdateAsync(review, changedDTO);

            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult<ReviewOutputDTO>.CompletedOperation(_mapper.Map<ReviewOutputDTO>(review));
            }
            catch(DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult<ReviewOutputDTO>.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return TResult<ReviewOutputDTO>.FailedOperation(errorCode.UnknownError);
            }


        }

        public async Task<TResult> DeleteReview(
            int reviewId, 
            int userId, 
            CancellationToken ct = default)
        {
            try
            {
                await _reviewRepository.GetAll()
                    .Where(c => c.id == reviewId && 
                           c.userid == userId)
                    .ExecuteDeleteAsync(ct);
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex) 
            {
                _logger.LogError(ex, "Ошибка бд при удалении отзыва");
                return TResult.FailedOperation(errorCode.DatabaseError);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при удалении отзыва");
                return TResult.FailedOperation(errorCode.UnknownError);
            }
          
        }

        public async Task<TResult<PagedResponseDTO<ReviewOutputDTO>>> GetReviewsByCourseId(
            int courseId, 
            SortingAndPaginationDTO sortingAndPagination, 
            CancellationToken ct = default)
        {
            var qwery = _reviewRepository
                .GetAllWithoutTracking()
                .Where(c => c.courseid == courseId);

            var entitylist = await qwery.GetWithPaginationAndSorting(sortingAndPagination).ToListAsync(ct);

            var dtolist = MapEntityListToDTOList(entitylist);

            return PageService.CreatePage(dtolist, sortingAndPagination, await qwery.CountAsync(ct));

        }


        private List<ReviewOutputDTO> MapEntityListToDTOList(List<ReviewEntities> entities)
        {
            var dtolist = entities.Select(c => new ReviewOutputDTO
            {
                id = c.id,
                content = c.content,
                createdat = c.createdat,
                courseId = c.courseid,
                review = c.review,
                userId = c.userid,
                dislikecount = c.dislikecount,
                likecount = c.likecount,
                lastchangedat = c.lastchangedat,
            }).ToList();
            return dtolist;
        }



        public async Task<TResult<PagedResponseDTO<ReviewOutputDTO>>> GetReviewsByUserId(
            int userid, 
            SortingAndPaginationDTO sortingAndPagination, 
            CancellationToken ct = default)
        {
            var qwery = _reviewRepository
              .GetAllWithoutTracking()
              .Where(c => c.userid  == userid);

            var entitylist = await qwery.GetWithPaginationAndSorting(sortingAndPagination).ToListAsync(ct);

            var dtolist = MapEntityListToDTOList(entitylist);

            return PageService.CreatePage(dtolist, sortingAndPagination, await qwery.CountAsync(ct));
        }

        public async Task<TResult> PostReview(ReviewICreateDTO review,
            int userid,
            CancellationToken ct = default)
        {

            var existsFromUser = await _courseRepository
                .GetAllWithoutTracking()
                .Where(c => c.id == review.courseid &&
                       c.creatorid == userid )
                .AnyAsync(ct);

            if(existsFromUser)
            {
                return TResult.FailedOperation(errorCode.CommentYourSelfCourseError);
            }

            var moreThanOneReview = await _reviewRepository
             .GetAllWithoutTracking()
             .Where(c => c.userid == userid)
             .AnyAsync();

            if(moreThanOneReview)
            {
                return TResult.FailedOperation(errorCode.MoreThanOne);
            }

            var entity = _mapper.Map<ReviewEntities>(review);
            entity.userid = userid;
            await _reviewRepository.Create(entity);
            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                if(pgEx.SqlState == 23503.ToString())
                {
                    return TResult.FailedOperation(errorCode.CoursesNotFoud);
                }

                _logger.LogError(ex, "Ошибка бд при создании отзыва");
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при создании отзыва");
                return TResult.FailedOperation(errorCode.UnknownError);
            }

        }
    }
}
