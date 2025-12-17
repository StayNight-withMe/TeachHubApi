using Core.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Model.TargetDTO.ReviewReaction;

using System.Security.Claims;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/review/reactiontype")]
    [Authorize]
    [Tags("Реакции на отзывы")]
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
