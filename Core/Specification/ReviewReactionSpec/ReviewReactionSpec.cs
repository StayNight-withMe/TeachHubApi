using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.ReviewReactionSpec
{
    public class ReviewReactionSpec : Specification<ReviewreactionEntity>
    {
        public ReviewReactionSpec(int reviewId, int userId)
        {
            Query.Where(r => r.reviewid == reviewId && 
                        r.userid == userId);
        }
    }
}
