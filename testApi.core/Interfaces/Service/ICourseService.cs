using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Courses.input;
using Core.Model.TargetDTO.Courses.output;
using Core.Model.TargetDTO.LessonFile.input;
using System.Security.Claims;

namespace Core.Interfaces.Service
{
    public interface ICourseService
    {
        public Task<TResult> CreateCourse(
            CreateCourseDTO courceDTO, 
            int id, 
            CancellationToken ct = default);
        public Task<TResult<PagedResponseDTO<CourseOutputDTO>>> GetAllCourse(
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default);
        public Task<TResult<PagedResponseDTO<CourseOutputDTO>>> GetUserCourses(
            int id, 
            string name, 
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default
            );
        public Task<TResult<CourseOutputDTO>> UpdateCourse(
            UpdateCourseDTO updateCourseDTO, 
            int userid,
            CancellationToken ct = default);
        public Task<TResult<PagedResponseDTO<CourseOutputDTO>>> SearchCourse(
            string search, 
            SortingAndPaginationDTO userSortingRequest,
            int userid = default,
            CancellationToken ct = default
            );
        public Task<TResult> RemoveCourse(
            int id, 
            ClaimsPrincipal user, 
            CancellationToken ct = default);

        Task<TResult> UploadFile(
            Stream stream,
              int userid,
              CancellationToken ct = default
              );



    }
}
