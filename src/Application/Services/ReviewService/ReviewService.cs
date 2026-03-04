using Application.Abstractions.Repository.Base;
using Application.Abstractions.Repository.Custom;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using Application.Utils.PageService;
using AutoMapper;
using Core.Common.Exeptions;
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Review.input;
using Core.Models.TargetDTO.Review.output;
using Core.Specification.CourseSpec;
using Core.Specification.ReviewSpec;
using Logger;
using Microsoft.Extensions.Logging;

namespace Application.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        private readonly IBaseRepository<CourseEntity> _courseRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ReviewService> _logger;

        private readonly IMapper _mapper;

        public ReviewService(
        IMapper mapper,
        IReviewRepository reviewRepository,
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
          .FirstOrDefaultAsync(new ReviewAuthorSpec(changedDTO.reviewid, userid), ct);

            if (review == null)
            {
                return TResult<ReviewOutputDTO>.FailedOperation(errorCode.ReviewNotFound);
            }

            review.lastchangedat = DateTime.UtcNow;
            await _reviewRepository.PartialUpdateAsync(review, changedDTO);

            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult<ReviewOutputDTO>
                    .CompletedOperation(_mapper.Map<ReviewOutputDTO>(review));
            }
            catch (DbUpdateException ex)
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
                var spec = new ReviewAuthorSpec(reviewId, userId);
                await _reviewRepository.ExecuteDeleteBySpecAsync(spec, ct);

                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }


        }

        public async Task<TResult<PagedResponseDTO<ReviewOutputDTO>>> GetReviewsByCourseId(
            int courseId,
            SortingAndPaginationDTO sortingAndPagination,
            CancellationToken ct = default)
        {
            var spec = new ReviewsByCourseSpec(courseId);

            var dtoList = await _reviewRepository.GetPagedReviewsDtoAsync(spec, sortingAndPagination, ct);

            var totalCount = await _reviewRepository.CountAsync(spec, ct);

            return PageService.CreatePage(dtoList, sortingAndPagination, totalCount);
        }


        //private List<ReviewOutputDTO> MapEntityListToDTOList(List<ReviewEntity> entities)
        //{
        //    var dtolist = entities.Select(c => new ReviewOutputDTO
        //    {
        //        id = c.id,
        //        content = c.content,
        //        createdat = c.createdat,
        //        courseId = c.courseid,
        //        review = c.review,
        //        userId = c.userid,
        //        dislikecount = c.dislikecount,
        //        likecount = c.likecount,
        //        lastchangedat = c.lastchangedat,
        //    }).ToList();
        //    return dtolist;
        //}



        public async Task<TResult<PagedResponseDTO<ReviewOutputDTO>>> GetReviewsByUserId(
     int userid,
     SortingAndPaginationDTO sortingAndPagination,
     CancellationToken ct = default)
        {

            var spec = new ReviewsByUserSpec(userid);

            var dtoList = await _reviewRepository.GetPagedReviewsDtoAsync(spec, sortingAndPagination, ct);

            var totalCount = await _reviewRepository.CountAsync(spec, ct);

            return PageService.CreatePage(dtoList, sortingAndPagination, totalCount);
        }

        public async Task<TResult> PostReview(
      ReviewICreateDTO review,
      int userid,
      CancellationToken ct = default)
        {

            var isCreator = await _courseRepository
                .AnyAsync(new CourseCreatorSpec(review.courseid, userid), ct);

            if (isCreator)
            {
                return TResult.FailedOperation(errorCode.CommentYourSelfCourseError);
            }

     
            var hasAlreadyReviewed = await _reviewRepository
                .AnyAsync(new UserAnyReviewSpec(userid), ct);

            if (hasAlreadyReviewed)
            {
                return TResult.FailedOperation(errorCode.MoreThanOne);
            }

    
            var entity = _mapper.Map<ReviewEntity>(review);
            entity.userid = userid;

   
            await _reviewRepository.AddAsync(entity, ct);

            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
            {
                if (ex.ErrorCode == "23503")
                {
                    return TResult.FailedOperation(errorCode.CoursesNotFound);
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
