using infrastructure.DataBase.Entitiеs;
using Ardalis.Specification;

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
