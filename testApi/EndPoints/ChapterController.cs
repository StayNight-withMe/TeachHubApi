using Asp.Versioning;
using testApi.WebUtils.JwtClaimUtil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Core.Common.Types.HashId;
using Application.Abstractions.Service;
using Core.Models.TargetDTO.Chapter.input;
using Core.Models.TargetDTO.Common.input;


namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/chapters")]
    [Tags("Разделы курсов")]
    [ApiVersion("1.0")]
    public class ChapterController : ControllerBase
    {

        private readonly IChapterService _chapterService;

        private readonly JwtClaimUtil _claims;

        public ChapterController(
            IChapterService chapterService,
            JwtClaimUtil claims
            ) 
        {
         _claims = claims;
         _chapterService = chapterService;
        }

        [OutputCache(PolicyName = "10min")]
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetChapter(
        [FromQuery] SortingAndPaginationDTO request,
        [FromRoute] Hashid courseId
        )
        {
            var result = await _chapterService.GetChaptersByCourseId(courseId, request);
            return await EntityResultExtensions.ToActionResult(result, this);
            
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateChapter([FromBody] CreateChapterDTO createChapterDTO)
        {
            var result = await _chapterService.Create(createChapterDTO, _claims.UserId);
            return await EntityResultExtensions.ToActionResult(result, this);
        }



        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateChapter( [FromBody] ChapterUpdateDTO chapterUpdateDTO )
        {
            var result = await _chapterService.UpdateChapter(chapterUpdateDTO, _claims.UserId);
            return await EntityResultExtensions.ToActionResult(result, this);

        }

        //[Authorize]
        //[HttpGet("my/{courseId}")]
        //public async Task<IActionResult> GetUserCourseChapter(
        //[FromQuery] UserSortingRequest request,
        //int courseId
        //)
        //{
            //var result = await _chapterService.GetChaptersByCourseIdAndUserId(courseId, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), request);

        //    if (result.IsCompleted)
        //    {
        //        return Ok(result.Value);
        //    }

        //    return EntityResultExtensions.ToActionResult(result, this);

        //}

        [Authorize]
        [HttpDelete("{chapterid}")]
        public async Task<IActionResult> DeleteChapter([FromRoute] Hashid chapterid)
        {
           var result = await _chapterService.DeleteChapter(chapterid, _claims.UserId);

            if (result.IsCompleted)
            {
                return Ok();
            }
            return Forbid();
        }



        [HttpDelete("admin/{chapterid}/{userid}")]
        [Authorize(Roles="admin")]
        public async Task<IActionResult> DeleteChapter(
             [FromRoute] Hashid chapterid,
             [FromRoute] Hashid userid
            )
        {
            var result = await _chapterService.DeleteChapter(chapterid, userid);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


    }
}
