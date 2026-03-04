using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.LessonStorageSpec
{
    public class LessonFilesByLessonSpec : Specification<LessonfileEntity>
    {
        public LessonFilesByLessonSpec(int lessonId)
        {
            Query.Where(f => f.lessonid == lessonId)
                 .AsNoTracking();
        }
    }
}
