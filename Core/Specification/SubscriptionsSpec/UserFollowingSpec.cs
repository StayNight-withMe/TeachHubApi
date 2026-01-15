using Ardalis.Specification;
using Core.Models.Entitiеs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specification.SubscriptionsSpec
{
    public class UserFollowingSpec : Specification<SubscriptionEntites>
    {
        public UserFollowingSpec(int userId)
        {
            Query.Where(s => s.followerid == userId)
                 .AsNoTracking()
                 .Include(s => s.following);
        }
    }
}
