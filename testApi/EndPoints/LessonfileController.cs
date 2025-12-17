using Amazon.Util.Internal;
using Core.Interfaces.Service;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.LessonFile.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/lessonsfiles")]
    [Tags("Файлы уроков")]
    public class LessonfileController : ControllerBase
    {
        private readonly ILessonStorageService _lessonStorageService;
        public LessonfileController(ILessonStorageService lessonStorageService)
        {
            _lessonStorageService = lessonStorageService;
        }

        [RequestSizeLimit(11 * 1024 * 1024)]
        [HttpPost("files")]
        [Authorize]
        public async Task<IActionResult> UploadLessonFile(
            IFormFile file,
            [FromForm] MetaDataDTO metaData,
            CancellationToken ct
            )
        {
            Stream stream = file.OpenReadStream();
            var result = await _lessonStorageService.UploadFile(
                stream,
                Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                metaData,
                file.ContentType,
                ct
                );
            return await EntityResultExtensions.ToActionResult(result, this);

        }


        
         [HttpGet("{lessonid}")]
         public async Task<IActionResult> GetLessonFile(
         [FromQuery] PaginationDTO pagination,
         int lessonid,
         CancellationToken ct
     )
        {
            var result =  await _lessonStorageService.GetLessonUrlFile(lessonid, pagination, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpDelete("{fileid}")]
        public async Task<IActionResult> DeleteLessonFile(
           int fileid,
           CancellationToken ct
       )
        {
            var result = await _lessonStorageService.DeleteLessonUrlFile(fileid, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

    }
}
