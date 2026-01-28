using Ardalis.Specification;

namespace Core.Specification.Profile
{
    public class UserProfileSpec : Specification<Core.Models.Entitiеs.ProfileEntity>
    {
        public UserProfileSpec(int userid, bool tracking = false)
        {
            if(!tracking)
            {
                Query.AsNoTracking();
            }
            Query.Where(p => p.userid == userid)
                 .Include(p => p.user);
        }
    }
}
