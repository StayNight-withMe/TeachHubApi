using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.ChapterSpec;

public class ChaptersByCourseSpec : Specification<ChapterEntity>
{
    public ChaptersByCourseSpec(int courseId, int userId)
    {
        
        Query.Include(c => c.course)
            .Where(ch => ch.courseid == courseId &&
                    ch.course.creatorid == userId)
             .AsNoTracking();
    }
}