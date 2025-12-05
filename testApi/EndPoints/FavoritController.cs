using Core.Interfaces.Service;
using Core.Model.TargetDTO.Common.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace testApi.EndPoints
{

    [Authorize]
    [ApiController]
    [Route("api/course/favorite")]
    public class FavoritController : ControllerBase
    {
        private readonly IFavoritService _favoritService;
        public FavoritController(IFavoritService favoritService) 
        {
            _favoritService = favoritService;
        }


        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] UserSortingRequest userSortingRequest )
        {
            var result = await _favoritService.GetFavorite(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), userSortingRequest);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpPost("{courseid}")]
        public async Task<IActionResult> Create(int courseid)
        {
            var result = await _favoritService.CreateFavorite(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), courseid);
            return await EntityResultExtensions.ToActionResult(result, this); 
        }

        [HttpDelete("{courseid}")]
        public async Task<IActionResult> Delete(int courseid)
        {
            var result = await _favoritService.DeleteFavorit(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), courseid);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

    }
}
