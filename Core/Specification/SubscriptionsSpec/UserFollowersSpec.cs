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
    public class UserFollowersSpec : Specification<SubscriptionEntites>
    {
        public UserFollowersSpec(int userId)
        {
            Query.Where(s => s.followingid == userId)
                 .AsNoTracking()
                 .Include(s => s.follower); 
        }
    }
}
