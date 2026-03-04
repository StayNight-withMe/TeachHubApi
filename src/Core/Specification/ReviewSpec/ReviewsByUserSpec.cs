using Ardalis.Specification;
using Core.Models.Entitiеs;


namespace Core.Specification.ReviewSpec
{
    public class ReviewsByUserSpec : Specification<ReviewEntity>
    {
        public ReviewsByUserSpec(int userId)
        {
            Query.Where(r => r.userid == userId)
                 .AsNoTracking();
        }
    }
}
