using Application.Abstractions.Service;
using Asp.Versioning;
using Core.Common.Types.HashId;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Lesson.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using testApi.WebUtils.JwtClaimUtil;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/lessons")]
    [Tags("Уроки")]
    [ApiVersion("1.0")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        private readonly JwtClaimUtil _claims;

        public LessonController(
            ILessonService lessonService,
            JwtClaimUtil claims
            )
        {
            _claims = claims;
            _lessonService = lessonService;
        }


        [HttpGet("{chapterid}")]
        [OutputCache(PolicyName = "10min")]
        public async Task<IActionResult> GetLesson(
            Hashid chapterid,
            [FromQuery] SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct
            )
        {
            var result = await _lessonService.GetLessonByChapterid(
                chapterid.Value, 
                userSortingRequest, 
                ct);
            return  await EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpGet("my{chapterid}")]
        [OutputCache(PolicyName = "1min")]
        [Authorize]
        public async Task<IActionResult> GetMyLesson(
        Hashid chapterid,
        [FromQuery] SortingAndPaginationDTO userSortingRequest,
        CancellationToken ct
    )
        {
            var result = await _lessonService.GetUnVisibleLessonByChapterid(
                _claims.UserId,
                chapterid,
                userSortingRequest,
                ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateLesson([FromBody] createLessonDTO lesson)
        {
            var result = await _lessonService.Create(
                lesson,
                _claims.UserId);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateLesson(
            [FromBody] LessonUpdateDTO lesson)
        {
            var result = await _lessonService.UpdateLesson(
                lesson,
                _claims.UserId);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpDelete("{lessonid}")]
        [Authorize]
        public async Task<IActionResult> DeleteLesson([FromRoute] Hashid lessonid)
        {
            var result = await _lessonService.DeleteLessonForUser(
                lessonid, 
                _claims.UserId);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpPatch("{lessonid}/visibility")]
        [Authorize]
        public async Task<IActionResult> SwitchVisible([FromRoute] Hashid lessonid)
        {
            var result = await _lessonService.SwitchVisible(
                lessonid, 
                _claims.UserId);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpDelete("admin/{lessonid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminDelete([FromRoute] Hashid lessonid)
        {
            var result = await _lessonService.DeleteLessonForAdmin(lessonid);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

    }
}
