using Core.Interfaces.Service;
using Core.Model.TargetDTO.Common.input;
using Microsoft.AspNetCore.Mvc;

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


    }
}
