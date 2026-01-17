using Asp.Versioning;
using testApi.WebUtils.JwtClaimUtil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Core.Common.Types.HashId;
using Application.Abstractions.Service;
using Core.Models.TargetDTO.Common.input;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/user/follows")]
    [Tags("Подписки")]
    [ApiVersion("1.0")]
    public class FollowController : ControllerBase
    {
        private readonly ISubscriptionService _followService;

        private readonly JwtClaimUtil _claims;

        public FollowController(
            ISubscriptionService subscriptionService,
            JwtClaimUtil claims
            )
        {
            _followService = subscriptionService;
            _claims = claims;
        }

        [Authorize]
        [HttpPost("{userid}")]
        public async Task<IActionResult> Addfollowing(
            [FromRoute] Hashid userid
            )
        {
            var result = await _followService.CreateSubscription(_claims.UserId, userid);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpGet("{userid}")]
        [OutputCache(PolicyName = "10min")]
        public async Task<IActionResult> GetUserFollowing(
          [FromRoute] Hashid userid, 
          [FromQuery] SortingAndPaginationDTO userSortingRequest)
        {
            var result = await _followService.GetUserFollowing(userid, userSortingRequest);
            return  await EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpGet("{userid}/followers")]
        public async Task<IActionResult> GetUserFollowers(
        [FromRoute] Hashid userid,
        [FromQuery] SortingAndPaginationDTO userSortingRequest)
        {
            var result = await _followService.GetUserFollowers(userid, userSortingRequest);
            return await EntityResultExtensions.ToActionResult(result, this);
        }



        [HttpGet]
        [Authorize]
        [OutputCache(PolicyName = "1min")]
        public async Task<IActionResult> GetMyFollowing(
        [FromQuery] SortingAndPaginationDTO userSortingRequest)
        {
            var result = await _followService.GetUserFollowing(_claims.UserId, userSortingRequest);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [Authorize]
        [HttpGet("followers")]
        [OutputCache(PolicyName = "1min")]
        public async Task<IActionResult> GetMyFollower(
            [FromQuery] SortingAndPaginationDTO userSortingRequest)
        {
            var result = await _followService.GetUserFollowers(_claims.UserId, userSortingRequest);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

        [Authorize]
        [HttpDelete("{following}")]
        public async Task<IActionResult> Deletefollowing([FromRoute] Hashid following)
        {
            var result = await _followService.DeleteSubscription(_claims.UserId, following);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

    }
}
