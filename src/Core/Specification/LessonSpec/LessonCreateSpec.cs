using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.LessonSpec
{
    public class LessonCreateSpec : Specification<LessonEntity>
    {
        public LessonCreateSpec(int order, string name, int chapterid)
        {
            Query.AsNoTracking()
                 .Where(c => c.chapterid == chapterid &&
                        (c.order == order ||
                        c.name == name ));
        }
    }
}
