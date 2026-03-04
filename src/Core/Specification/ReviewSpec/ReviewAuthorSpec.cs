using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.ReviewSpec
{
    public class ReviewAuthorSpec : Specification<ReviewEntity>
    {
        public ReviewAuthorSpec(int reviewId, int userId)
        {
            Query.Where(r => r.id == reviewId && r.userid == userId)
                 .AsNoTracking(); 
        }
    }
}
