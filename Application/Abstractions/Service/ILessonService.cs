using Core.Model.BaseModel.Lesson;
using Core.Model.TargetDTO.Chapter.input;
using Core.Model.TargetDTO.Chapter.output;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Lesson.input;
using Core.Models.TargetDTO.Lesson.output;

namespace Application.Abstractions.Service
{
    public interface ILessonService
    {
        Task<TResult> Create(
            createLessonDTO lesson, 
            int userid, 
            CancellationToken ct = default);
        Task<TResult<lessonOutputDTO>> UpdateLesson(
            LessonUpdateDTO newlesson, 
            int userid, 
            CancellationToken ct = default);
        Task<TResult<PagedResponseDTO<lessonOutputDTO>>> GetLessonByChapterid(
            int chapterid, 
            SortingAndPaginationDTO userSortingRequest, 
            CancellationToken ct = default);
        Task<TResult> DeleteLessonForUser(
            int lessonid, 
            int userid, 
            CancellationToken ct = default);
        Task<TResult> DeleteLessonForAdmin(
            int lessonid, 
            CancellationToken ct = default );
        Task<TResult> SwitchVisible(
            int lessonid, 
            int userid, 
            CancellationToken ct = default);

        Task<TResult<PagedResponseDTO<LessonUserOutputDTO>>> GetUnVisibleLessonByChapterid(
            int userid,
            int chapterid,
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default);
    }
}

