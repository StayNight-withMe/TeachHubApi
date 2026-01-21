using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.LessonSpec
{
    public class LessonByUserSpec : Specification<LessonEntity>
    {
        public LessonByUserSpec(int userid, int lessonid, bool tracking = false)
        {

            if (!tracking)
            {
                Query.AsNoTracking();
            }

            Query
                .Include(lesson => lesson.course)
                .Where(lesson => lesson.id == lessonid &&
                lesson.course.creatorid == userid);
        }
    }
}
