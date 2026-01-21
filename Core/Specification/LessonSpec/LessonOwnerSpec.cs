using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.LessonSpec
{
    /// <summary>
    /// Спецификация для проверки: принадлежит ли урок пользователю (через создателя курса)
    /// </summary>
    public class LessonOwnerSpec : Specification<LessonEntity, int>
    {
        public LessonOwnerSpec(int lessonId, int userId)
        {
            Query.AsNoTracking()
                 .Include(c => c.course)
                 .Where(l => l.id == lessonId && 
                        l.course.creatorid == userId)
                 .Select(l => l.id); 
        }
    }
}
