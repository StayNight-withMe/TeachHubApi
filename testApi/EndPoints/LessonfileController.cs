using Amazon.Util.Internal;
using Application.Abstractions.Service;
using Asp.Versioning;
using Core.Common.Types.HashId;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.LessonFile.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using testApi.WebUtils.JwtClaimUtil;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/lessonsfiles")]
    [Tags("Файлы уроков")]
    [ApiVersion("1.0")]
    public class LessonfileController : ControllerBase
    {
        private readonly ILessonStorageService _lessonStorageService;

        private readonly JwtClaimUtil _claims;

        public LessonfileController(
            ILessonStorageService lessonStorageService,
            JwtClaimUtil claim
            )
        {
            _lessonStorageService = lessonStorageService;
            _claims = claim;
        }

        [RequestSizeLimit(11 * 1024 * 1024)]
        [HttpPost("files")]
        [Authorize]
        public async Task<IActionResult> UploadLessonFile(
            IFormFile file,
            [FromForm] MetaDataLessonDTO metaData,
            CancellationToken ct
            )
        {
            Stream stream = file.OpenReadStream();
            var result = await _lessonStorageService.UploadFile(
                stream,
                _claims.UserId,
                metaData,
                file.ContentType,
                ct
                );
            return await EntityResultExtensions.ToActionResult(result, this);

        }


        
         [HttpGet("{lessonid}")]
         public async Task<IActionResult> GetLessonFile(
         [FromQuery] PaginationDTO pagination,
         [FromRoute] Hashid lessonid,
         CancellationToken ct
     )
        {
            var result =  await _lessonStorageService.GetLessonUrlFile(lessonid, pagination, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [HttpDelete("{fileid}")]
        public async Task<IActionResult> DeleteLessonFile(
           [FromRoute] Hashid fileid,
           CancellationToken ct
       )
        {
            var result = await _lessonStorageService.DeleteLessonUrlFile(fileid, _claims.UserId, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }

    }
}
