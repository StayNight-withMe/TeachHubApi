using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.ChapterSpec;

public class ChaptersByCourseSpec : Specification<ChapterEntity>
{
    public ChaptersByCourseSpec(int courseId, int userId)
    {
        Query.Where(ch => ch.courseid == courseId && ch.course.creatorid == userId)
             .AsNoTracking();
    }
}