using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.SubscriptionsSpec
{
    public class UserFollowingSpec : Specification<SubscriptionEntity>
    {
        public UserFollowingSpec(int userId)
        {
            Query.Where(s => s.followerid == userId)
                 .AsNoTracking()
                 .Include(s => s.following);
        }
    }
}
