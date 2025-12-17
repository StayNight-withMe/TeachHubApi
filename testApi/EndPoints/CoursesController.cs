using Applcation.Service.chapterService;
using Core.Interfaces.Service;
using Core.Model.TargetDTO.Chapter.input;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Courses.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;
using System.Threading.Tasks;

namespace testApi.EndPoints
{

    [ApiController]
    [Route("api/courses")]
    [Tags("Курсы")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService) 
        {
            _courseService = courseService;        
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateCourse([FromBody] UpdateCourseDTO course)
        {
            var result = await _courseService.UpdateCourse(course, Convert.ToInt32(User.FindFirst("id").Value));
            return await EntityResultExtensions.ToActionResult(result, this);

        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDTO courceDTO)
        {
            var result = await _courseService.CreateCourse(courceDTO, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ));
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

            if(searchText == null)
            {
                var result = await _courseService.GetAllCourse(userSortingRequest, ct);
                return await EntityResultExtensions.ToActionResult(result, this);
            }

            int userid = default;

            if((Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), User.FindFirstValue(ClaimTypes.Name)) != default)
            {
                userid = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
   
            var result1 = await _courseService.SearchCourse(searchText, userSortingRequest);
            return await EntityResultExtensions.ToActionResult(result1, this);
        }




        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Remove(
            int id,
            CancellationToken ct
            )
        {
            var result = await _courseService.RemoveCourse(id, User, ct);
            return await EntityResultExtensions.ToActionResult(result, this);
        }


        [Authorize]
        [HttpGet("my")]
        [OutputCache(PolicyName = "10min")]
        public async Task<IActionResult> GetUserCourses([FromQuery] SortingAndPaginationDTO userSortingRequest)
        {
            var result = await _courseService.GetUserCourses(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), User.FindFirstValue(ClaimTypes.Name), userSortingRequest);
            return await EntityResultExtensions.ToActionResult(result, this);   
        }






    }
}
