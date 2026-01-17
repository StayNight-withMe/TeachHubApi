using Application.Abstractions.Service;
using Asp.Versioning;
using Core.Common.Types.HashId;
using Core.Models.TargetDTO.Common.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using testApi.WebUtils.JwtClaimUtil;

namespace testApi.EndPoints
{

    [Authorize]
    [ApiController]
    [Route("api/favorites")]
    [Tags("Избранное")]
    [ApiVersion("1.0")]
    public class FavoritController : ControllerBase
    {
        private readonly IFavoritService _favoritService;

        private readonly JwtClaimUtil _jwtClaimUtil;
        public FavoritController(
            IFavoritService favoritService,
            JwtClaimUtil jwtClaimUtil
            ) 
        {
            _favoritService = favoritService;
            _jwtClaimUtil = jwtClaimUtil;
        }


        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] SortingAndPaginationDTO userSortingRequest )
        {
            var result = await _favoritService.GetFavorite(_jwtClaimUtil.UserId, userSortingRequest);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpPost("{courseid}")]
        public async Task<IActionResult> Create([FromRoute] Hashid courseid)
        {
            var result = await _favoritService.CreateFavorite(_jwtClaimUtil.UserId, courseid);
            return await EntityResultExtensions.ToActionResult(result, this); 
        }

        [HttpDelete("{courseid}")]
        public async Task<IActionResult> Delete([FromRoute] Hashid courseid)
        {
            var result = await _favoritService.DeleteFavorit(_jwtClaimUtil.UserId, courseid);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

    }
}
