using Application.Abstractions.Repository.Base;
using Ardalis.Specification;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Lesson.output;

namespace Application.Abstractions.Repository.Custom
{
    public interface ILessonRepository : IBaseRepository<LessonEntity>
    {
        public Task<List<lessonOutputDTO>> GetLessonByChapterid(
            int chapterid,
            SortingAndPaginationDTO userSortingRequest,
            ISpecification<LessonEntity> spec,
            CancellationToken ct = default
            );

        public Task<List<LessonUserOutputDTO>> GetLessonUserByChapterid(
            int chapterid,
            SortingAndPaginationDTO userSortingRequest,
            ISpecification<LessonEntity> spec,
            CancellationToken ct = default
            );
        //     var qwery = _lessonRepository.GetAllWithoutTracking()
        //.Where(c => c.chapterid == chapterid &&
        //       c.isvisible == true);
        //     var lessons = await qwery
        //         .GetWithPaginationAndSorting(userSortingRequest, "isvisible", "chapterid", "id")
        //         .ToListAsync(ct);
        //  List<lessonOutputDTO> Outlist = lessons.Select(c => new lessonOutputDTO
        //  {
        //      name = c.name,
        //      id = c.id,
        //      order = (int)c.order
        //  }
        //).ToList()
    }
}
