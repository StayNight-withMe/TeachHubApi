using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specifications.Chapters;

public class ChapterDeleteSpec : Specification<ChapterEntity>
{
    public ChapterDeleteSpec(int chapterId, int? userId = null)
    {
        var query = Query.Where(c => c.id == chapterId);

        if (userId.HasValue && userId != default)
        {
            query.Where(c => c.course.creatorid == userId.Value);
        }
    }
}

