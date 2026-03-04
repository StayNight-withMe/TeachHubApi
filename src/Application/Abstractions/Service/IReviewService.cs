using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Review.input;
using Core.Models.TargetDTO.Review.output;


namespace Application.Abstractions.Service
{
    public interface IReviewService
    {
        Task<TResult> PostReview(
            ReviewICreateDTO review, 
            int userid,
            CancellationToken ct = default
            );
        Task<TResult<PagedResponseDTO<ReviewOutputDTO>>> GetReviewsByCourseId(
            int courseId,
            SortingAndPaginationDTO sortingAndPagination,
            CancellationToken ct = default
            );

        Task<TResult<PagedResponseDTO<ReviewOutputDTO>>> GetReviewsByUserId(
            int userid,
            SortingAndPaginationDTO sortingAndPagination,
            CancellationToken ct = default
            );

        Task<TResult<ReviewOutputDTO>> UpdateReview(
            ReviewChangedDTO changedDTO, 
            int userid,
            CancellationToken ct = default
            );


        Task<TResult> DeleteReview(
            int reviewId,
            int userId,
            CancellationToken ct = default
            );

        


    }
}
