using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.FavoriteSpec
{
    public class UsersFavoriteSpec : Specification<FavoritEntity>
    {
        public UsersFavoriteSpec(
            int courseid, 
            int userid, 
            bool tracking = false)
        {
            if(!tracking)
            {
                Query.AsNoTracking();
            }

            Query.Where(c => c.courseid == courseid &&
               c.userid == userid);


        }
    }
}
