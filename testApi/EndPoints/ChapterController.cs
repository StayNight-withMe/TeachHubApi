using Core.Interfaces.Service;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Chapter.input;
using Core.Model.TargetDTO.Common.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;


namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/courses/chapter")]
    public class ChapterController : ControllerBase
    {

        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService) 
        {
         _chapterService = chapterService;
        }

        [OutputCache(PolicyName = "10min")]
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetChapter(
        [FromQuery] SortingAndPaginationDTO request,
        int courseId
        )
        {
            var result = await _chapterService.GetChaptersByCourseId(courseId, request);
            return await EntityResultExtensions.ToActionResult(result, this);
            
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateChapter([FromBody] CreateChapterDTO createChapterDTO)
        {
            var result = await _chapterService.Create(createChapterDTO, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return await EntityResultExtensions.ToActionResult(result, this);
        }



        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateChapter( [FromBody] ChapterUpdateDTO chapterUpdateDTO )
        {
            var result = await _chapterService.UpdateChapter(chapterUpdateDTO, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            if (result.IsCompleted)
            {
                return Ok(result.Value);
            }

            return await EntityResultExtensions.ToActionResult(result, this);

        }

        //[Authorize]
        //[HttpGet("my/{courseId}")]
        //public async Task<IActionResult> GetUserCourseChapter(
        //[FromQuery] UserSortingRequest request,
        //int courseId
        //)
        //{
        //    var result = await _chapterService.GetChaptersByCourseIdAndUserId(courseId, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), request);

        //    if (result.IsCompleted)
        //    {
        //        return Ok(result.Value);
        //    }

        //    return EntityResultExtensions.ToActionResult(result, this);

        //}

        [Authorize]
        [HttpDelete("{chapterid}")]
        public async Task<IActionResult> DeleteChapter(int chapterid)
        {
           var result = await _chapterService.DeleteChapter(chapterid, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            if (result.IsCompleted)
            {
                return Ok();
            }
            return Forbid("отсутствуют права на удаление");
        }



        [HttpDelete("admin/{chapterid}/{userid}")]
        [Authorize(Roles="admin")]
        public async Task<IActionResult> DeleteChapter(
             int chapterid,
             int userid
            )
        {
            var result = await _chapterService.DeleteChapter(chapterid, userid);

            if (result.IsCompleted)
            {
                return Ok();
            }
            return await EntityResultExtensions.ToActionResult(result, this);
        }


    }
}
