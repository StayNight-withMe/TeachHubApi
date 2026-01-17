using Application.Abstractions.Service;
using Asp.Versioning;
using Core.Common.EnumS;
using Core.Common.Types.HashId;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Courses.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using testApi.WebUtils.JwtClaimUtil;

namespace testApi.EndPoints
{

    [ApiController]
    [Route("api/courses")]
    [Tags("Курсы")]
    [ApiVersion("1.0")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        private readonly JwtClaimUtil _claims;

        public CoursesController(
            ICourseService courseService,
            JwtClaimUtil claims
            ) 
        {
            _courseService = courseService;
            _claims = claims;
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateCourse(
            [FromBody] UpdateCourseDTO course,
            CancellationToken ct
            )
        {
            var result = await _courseService.UpdateCourse(course, _claims.UserId, ct);
            return await EntityResultExtensions.ToActionResult(result, this);

        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCourse(
            [FromBody] CreateCourseDTO courceDTO, 
            CancellationToken ct
            )
        {
            var result = await _courseService.CreateCourse(courceDTO, _claims.UserId, ct);
            return await EntityResultExtensions.ToActionResult(result, this);    
        }


        [HttpGet]
        [OutputCache(PolicyName = "20min")]
        public async Task<IActionResult> SearchCourse(
            [FromQuery] SortingAndPaginationDTO userSortingRequest,
            [FromQuery] string searchText = null,
            CancellationToken ct = default
            )
        {

            if(string.IsNullOrEmpty(searchText))
            {
                return BadRequest();
            }

            int UserId = _claims.UserId;

            //if((Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), User.FindFirstValue(ClaimTypes.Name)) != default)
            //{
            //    userid = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            //}

            Console.WriteLine(UserId);

            var result1 = await _courseService.SearchCourse(searchText, userSortingRequest, UserId, ct);
            return await EntityResultExtensions.ToActionResult(result1, this);
        }


        [RequestSizeLimit(11 * 1024 * 1024)]
        [HttpPut("icon")]
        [Authorize]
        public async Task<IActionResult> UploadCourseIcon(
            IFormFile? file,
            [FromForm] CourseSetImageDTO setImageDTO,
            CancellationToken ct
            )
        {

            

            var allowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg",
                "image/png",
                "image/gif",
                "image/webp",
                "image/bmp"
            };


            Stream? stream = null;

            string? ContentType = null;

            if (
                file != null && 
                file.Length != 0 
                && setImageDTO.setstatus == SetImageStatus.Upload)
            {

                if (!allowedTypes.Contains(file.ContentType))
                {
                    return BadRequest(new { code = "ContentType is not on the allowed list" });
                }

                stream = file.OpenReadStream();

                ContentType = file.ContentType;
            }

            var result = await _courseService.SetImgFile(
                stream,
                _claims.UserId,
                setImageDTO,
                ContentType,
                ct
                );

            return await EntityResultExtensions.ToActionResult(result, this);

        }






        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Remove(
            [FromRoute] Hashid id,
            CancellationToken ct
            )
        {
            var result = await _courseService.RemoveCourse(id, User, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [Authorize]
        [HttpGet("${courseid}")]
        [OutputCache(PolicyName = "10min")]
        public async Task<IActionResult> GetUserCourses(
        [FromQuery] SortingAndPaginationDTO userSortingRequest,
        CancellationToken ct,
        [FromRoute] Hashid courseid
    )
        {
            var result = await _courseService.GetUserCourses(courseid, userSortingRequest, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [Authorize]
        [HttpGet("my")]
        [OutputCache(PolicyName = "10min")]
        public async Task<IActionResult> GetMyCourses(
            [FromQuery] SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct
            )
        {
            var result = await _courseService.GetUserCourses(_claims.UserId, userSortingRequest, ct);
            return await EntityResultExtensions.ToActionResult(result, this);   
        }






    }
}
