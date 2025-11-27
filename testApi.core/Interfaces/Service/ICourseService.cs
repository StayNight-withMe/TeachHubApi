using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Courses.input;
using Core.Model.TargetDTO.Courses.output;
using System.Security.Claims;
using Core.Model.TargetDTO.Common.output;

namespace Core.Interfaces.Service
{
    public interface ICourseService
    {
        public Task<TResult> CreateCourse(CreateCourseDTO courceDTO, int id);
        public Task<TResult<PagedResponseDTO<CourseOutputDTO>>> GetAllCourse(UserSortingRequest userSortingRequest);
        public Task<TResult<PagedResponseDTO<CourseOutputDTO>>> GetUserCourses(int id, string name, UserSortingRequest userSortingRequest);
        public Task<TResult<CourseOutputDTO>> UpdateCourse(UpdateCourseDTO updateCourseDTO, int userid);
        public Task<TResult<PagedResponseDTO<CourseOutputDTO>>> SearchCourse(string search, UserSortingRequest userSortingRequest);
        public Task<TResult> RemoveCourse(int id, ClaimsPrincipal user);
    }
}
