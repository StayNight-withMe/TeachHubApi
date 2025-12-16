using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Review.input;
using Core.Model.TargetDTO.Review.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface IReviewService
    {
        Task<TResult> PostReview(
            ReviewInputDTO review, 
            int userid,
            CancellationToken ct = default);
        Task<TResult<PagedResponseDTO<ReviewOutputDTO>>> GetReviewsByCourseId(
            int courseId,
            SortingAndPaginationDTO sortingAndPagination,
            CancellationToken ct = default
            );

        Task<TResult> DeleteReview(
            int reviewId,
            int userId,
            CancellationToken ct = default
            );


    }
}
