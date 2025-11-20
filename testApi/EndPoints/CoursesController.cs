using Applcation.Service.chapterService;
using Core.Interfaces.Service;
using Core.Model.TargetDTO.Chapter.input;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Courses.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace testApi.EndPoints
{

    [ApiController]
    [Route("api/courses")]
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
            return EntityResultExtensions.ToActionResult(result, this);

        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDTO courceDTO)
        {
            var result = await _courseService.CreateCourse(courceDTO, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ));
            return EntityResultExtensions.ToActionResult(result, this);    
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _courseService.RemoveCourse(id, User);
            return EntityResultExtensions.ToActionResult(result, this);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllCources([FromQuery] UserSortingRequest userSortingRequest)
        {
            var result = await _courseService.GetAllCourse(userSortingRequest);
            if(result.IsCompleted)
            {
                return Ok(result.Value);
            }
            else
            {
                return EntityResultExtensions.ToActionResult(result, this);
            }
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetUserCourses([FromQuery] UserSortingRequest userSortingRequest)
        {
            
            var result = await _courseService.GetUserCourses(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), User.FindFirstValue(ClaimTypes.Name), userSortingRequest);
            if (result.IsCompleted)
            {
                return Ok(result.Value);
            }
            else
            {
                return EntityResultExtensions.ToActionResult(result, this);
            }
        }






    }
}
