using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.ChapterSpec;

public class ChapterWithAccessSpec : Specification<ChapterEntity>
{
    public ChapterWithAccessSpec(int chapterId, int userId)
    {
        Query.Where(ch => ch.id == chapterId && 
                    ch.course.creatorid == userId)
             .Include(ch => ch.course); 
    }
}