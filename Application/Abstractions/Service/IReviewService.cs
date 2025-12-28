using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Review.input;
using Core.Model.TargetDTO.Review.output;


namespace Core.Interfaces.Service
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
