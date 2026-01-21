using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.ReviewSpec
{
    public class UserAnyReviewSpec : Specification<ReviewEntity>
    {
        public UserAnyReviewSpec(int userId)
        {
            Query.Where(r => r.userid == userId)
                 .AsNoTracking();
        }
    }
}
