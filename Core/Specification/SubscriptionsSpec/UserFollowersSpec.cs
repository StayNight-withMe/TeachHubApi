using Ardalis.Specification;
using Core.Models.Entitiеs;


namespace Core.Specification.SubscriptionsSpec
{
    public class UserFollowersSpec : Specification<SubscriptionEntity>
    {
        public UserFollowersSpec(int userId)
        {
            Query.Where(s => s.followingid == userId)
                 .AsNoTracking()
                 .Include(s => s.follower); 
        }
    }
}
