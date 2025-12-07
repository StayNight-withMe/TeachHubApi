using Core.Model.BaseModel.Lesson;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Chapter.input;
using Core.Model.TargetDTO.Chapter.output;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Lesson.input;
using Core.Model.TargetDTO.Lesson.output;

namespace Core.Interfaces.Service
{
    public interface ILessonService
    {
        Task<TResult> Create(createLessonDTO lesson, int userid);
        Task<TResult<lessonOutputDTO>> UpdateLesson(LessonUpdateDTO newlesson, int userid);
        Task<TResult<PagedResponseDTO<lessonOutputDTO>>> GetLessonByChapterid(int chapterid, SortingAndPaginationDTO userSortingRequest);
        Task<TResult> DeleteLessonForUser(int lessonid, int userid);
        Task<TResult> DeleteLessonForAdmin(int lessonid);
        Task<TResult> SwitchVisible(int lessonid, int userid);
    }
}

