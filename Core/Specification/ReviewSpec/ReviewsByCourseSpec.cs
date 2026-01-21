using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.ReviewSpec
{
    public class ReviewsByCourseSpec : Specification<ReviewEntity>
    {
        public ReviewsByCourseSpec(int courseId)
        {
            Query.Where(r => r.courseid == courseId)
                 .AsNoTracking();
        }
    }
}
