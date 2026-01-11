using Ardalis.Specification;
using infrastructure.DataBase.Entitiеs;
namespace Core.Specification.FavoriteSpec
{
    public class UserFavorite : Specification<FavoritEntity>
    {
        public UserFavorite(int userid, bool include = false)
        {
            if(include)
            {
                Query.Include(c => c.course)
               .Include(c => c.user);
            }


         Query
              .Where(c => c.userid == userid);
        }
    }
}
