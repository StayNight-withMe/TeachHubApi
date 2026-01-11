using Application.Abstractions.Service;
using Asp.Versioning;
using Core.Common.Types.HashId;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Review.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using testApi.WebUtils.JwtClaimUtil;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/reviews")]
    [Tags("Отзывы")]
    [ApiVersion("1.0")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        private readonly JwtClaimUtil _claims;

        public ReviewController(
            IReviewService reviewService,
            JwtClaimUtil claims
            )
        {
            _claims = claims;
            _reviewService = reviewService;
        }

        
        [HttpGet("{courseid}")]
        public async Task<IActionResult> GetReviews(
            [FromRoute] Hashid courseid,
            [FromQuery] SortingAndPaginationDTO sort,
            CancellationToken ct
            )
        {
            var result = await _reviewService.GetReviewsByCourseId(courseid, sort, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyReviews(
         [FromQuery] SortingAndPaginationDTO sort,
         CancellationToken ct)
        {
            var result = await _reviewService.GetReviewsByUserId(_claims.UserId, sort, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }




        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostReviews(
            [FromRoute] Hashid courseid,
            [FromBody]ReviewICreateDTO reviewInputDTO,
            CancellationToken ct)
        {
            var result = await _reviewService.PostReview(reviewInputDTO, _claims.UserId, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpDelete("{reviewid}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(
            [FromRoute] Hashid reviewid,
            CancellationToken ct
            )
        {
            var result = await _reviewService.DeleteReview(reviewid, _claims.UserId, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpPatch("{reviewid}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(
        [FromBody] ReviewChangedDTO review,
        CancellationToken ct
    )
        {
            var result = await _reviewService.UpdateReview(review, _claims.UserId, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }



    }
}
