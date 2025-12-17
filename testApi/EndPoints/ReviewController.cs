using Core.Interfaces.Service;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Review.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/review")]
    [Tags("Отзывы")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(
            IReviewService reviewService
            )
        {
            _reviewService = reviewService;
        }

        
        [HttpGet("{courseid}")]
        public async Task<IActionResult> GetReviews(
            int courseid,
            [FromQuery] SortingAndPaginationDTO sort,
            CancellationToken ct
            )
        {
            var result = await _reviewService.GetReviewsByCourseId(courseid, sort, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpGet]
        public async Task<IActionResult> GetMyReviews(
         [FromQuery] SortingAndPaginationDTO sort,
         CancellationToken ct
     )
        {
            var result = await _reviewService.GetReviewsByUserId(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), sort, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }




        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostReviews(
            int courseid,
            [FromQuery]ReviewICreateDTO reviewInputDTO,
            CancellationToken ct)
        {
            var result = await _reviewService.PostReview(reviewInputDTO, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpDelete("{reviewid}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(
            int reviewid,
            CancellationToken ct
            )
        {
            var result = await _reviewService.DeleteReview(reviewid, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpPatch("{reviewid}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(
        [FromQuery] ReviewChangedDTO review,
        CancellationToken ct
    )
        {
            var result = await _reviewService.UpdateReview(review, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }



    }
}
