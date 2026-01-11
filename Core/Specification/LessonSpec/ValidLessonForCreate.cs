using Core.Models.Entitiеs;
using Ardalis.Specification;

namespace Core.Specification.LessonSpec
{
    public class ValidLessonForCreate : Specification<ChapterEntity>
    {
        public ValidLessonForCreate(int order, string lessonName)
        {
            Query.AsNoTracking();
            Query.Where(c => c.order == order &&
                   c.name == lessonName);
        }
    }
}
