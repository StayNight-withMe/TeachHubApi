using Core.Interfaces.Service;
using Core.Interfaces.Utils;
using Core.Model.TargetDTO.Common.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/user/follows")]
    [Tags("Подписки")]
    public class FollowController : ControllerBase
    {
        private readonly ISubscriptionService _followService;
        public FollowController(
            ISubscriptionService subscriptionService
            )
        {
            _followService = subscriptionService;
        }

        [Authorize]
        [HttpPost("{userid}")]
        public async Task<IActionResult> Addfollowing(int userid)
        {
            var result = await _followService.CreateSubscription(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), userid);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpGet("{userid}")]
        [OutputCache(PolicyName = "10min")]
        public async Task<IActionResult> GetUserFollowing(
          int userid, 
          [FromQuery] SortingAndPaginationDTO userSortingRequest)
        {
            var result = await _followService.GetUserFollowing(userid, userSortingRequest);
            return  await EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpGet("{userid}/followers")]
        public async Task<IActionResult> GetUserFollowers(
        int userid,
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
            var result = await _followService.GetUserFollowing(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), userSortingRequest);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [Authorize]
        [HttpGet("followers")]
        [OutputCache(PolicyName = "1min")]
        public async Task<IActionResult> GetMyFollower(
            [FromQuery] SortingAndPaginationDTO userSortingRequest)
        {
            var result = await _followService.GetUserFollowers(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), userSortingRequest);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

        [Authorize]
        [HttpDelete("{following}")]
        public async Task<IActionResult> Deletefollowing(int following)
        {
            var result = await _followService.DeleteSubscription(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), following);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

    }
}
