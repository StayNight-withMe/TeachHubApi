using Application.Abstractions.Service;
using Asp.Versioning;
using Core.Models.TargetDTO.ReviewReaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using testApi.WebUtils.JwtClaimUtil;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/reviews/reactions")]
    [Authorize]
    [Tags("Реакции на отзывы")]
    [ApiVersion("1.0")]
    public class ReviewReactionController : ControllerBase
    {
        private readonly IReviewReactionService _reviewReactionService;  
        
        private readonly JwtClaimUtil _claims;

        public ReviewReactionController(
            IReviewReactionService reviewReactionService,
            JwtClaimUtil claims
            )
        {
            _claims = claims;
            _reviewReactionService = reviewReactionService;
        }


        [HttpPut]
        public async Task<IActionResult> PutReaction(
            [FromQuery] ReviewReactionInputDTO reactionDTO,
            CancellationToken ct)
        {
            var result = await _reviewReactionService.PutReaction(
                reactionDTO,
                _claims.UserId,
                ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }



    }
}
