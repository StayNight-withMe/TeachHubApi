using Core.Interfaces.Service;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Lesson.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;
using System.Threading.Tasks;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/courses/chapter/lesson")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }


        [HttpGet("{chapterid}")]
        [OutputCache(PolicyName = "10min")]
        public async Task<IActionResult> GetLesson(int chapterid,
            [FromQuery] UserSortingRequest userSortingRequest)
        {
            var result = await _lessonService.GetLessonByChapterid(chapterid, userSortingRequest);
            return EntityResultExtensions.ToActionResult(result, this);
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateLesson([FromBody] createLessonDTO lesson)
        {
            var result = await _lessonService.Create(lesson, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateLesson([FromBody] LessonUpdateDTO lesson)
        {
            var result = await _lessonService.UpdateLesson(lesson, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpDelete("{lessonid}")]
        [Authorize]
        public async Task<IActionResult> DeleteLesson(int lessonid)
        {
            var result = await _lessonService.DeleteLessonForUser(lessonid, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpPatch("{lessonid}/visibility")]
        [Authorize]
        public async Task<IActionResult> SwitchVisible(int lessonid)
        {
            var result = await _lessonService.SwitchVisible(lessonid, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpDelete("admin/{lessonid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminDelete(int lessonid)
        {
            var result = await _lessonService.DeleteLessonForAdmin(lessonid);
            return EntityResultExtensions.ToActionResult(result, this);
        }

    }
}
