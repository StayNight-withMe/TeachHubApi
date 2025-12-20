using Asp.Versioning;
using Core.Interfaces.Service;
using Core.Model.TargetDTO.ReviewReaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public ReviewReactionController(
            IReviewReactionService reviewReactionService
            )
        {
            _reviewReactionService = reviewReactionService;
        }


        [HttpPut]
        public async Task<IActionResult> PutReaction(
            [FromQuery] ReviewReactionInputDTO reactionDTO,
            CancellationToken ct)
        {
            var result = await _reviewReactionService.PutReaction(
                reactionDTO,
                Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }



    }
}
