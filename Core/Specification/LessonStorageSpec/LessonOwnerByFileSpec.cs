using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.LessonStorageSpec
{
    public class LessonOwnerByFileSpec : Specification<LessonEntity>
    {
        public LessonOwnerByFileSpec(int lessonId, int userId)
        {
            Query.Where(l => l.id == lessonId && l.course.creatorid == userId)
                 .Include(l => l.course) // Необходим для доступа к creatorid
                 .AsNoTracking();
        }
    }
}
