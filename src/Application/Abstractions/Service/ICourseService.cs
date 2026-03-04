using Core.Common.EnumS;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Courses.input;
using Core.Models.TargetDTO.Courses.output;
using System.Security.Claims;

namespace Application.Abstractions.Service
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
            //string name, 
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
            int courseid,
            int userId,
            AllRole role,
            CancellationToken ct = default);

        Task<TResult<SetImageOutPutDTO>> SetImgFile(
            Stream stream,
            int userid,
            CourseSetImageDTO courseSetImageDTO,  
            string contentType,
            CancellationToken ct = default
              );



    }
}
