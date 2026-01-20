using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specifications.Chapters;

public class ChaptersByCoursePublicSpec : Specification<ChapterEntity>
{
    public ChaptersByCoursePublicSpec(int courseId)
    {
        Query.Where(ch => ch.courseid == courseId)
             .AsNoTracking();
    }
}