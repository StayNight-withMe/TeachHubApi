using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.LessonStorageSpec
{
    public class LessonFileCreateSpec : Specification<LessonfileEntity>
    {
        public LessonFileCreateSpec(int lessonid, string filename, int order)
        {
            Query.AsNoTracking()
                 .Where(c => c.lessonid == lessonid &&
                 (c.filename == filename || 
                  c.order == order));
        }
    }
}
