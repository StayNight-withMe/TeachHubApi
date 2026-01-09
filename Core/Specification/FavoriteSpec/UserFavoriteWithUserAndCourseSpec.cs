using Ardalis.Specification;
using infrastructure.DataBase.Entitiеs;
namespace Core.Specification.FavoriteSpec
{
    public class UserFavoriteWithUserAndCourseSpec : Specification<FavoritEntity>
    {
        public UserFavoriteWithUserAndCourseSpec(int userid)
        {
         Query.Include(c => c.course)
              .Include(c => c.user)
              .Where(c => c.userid == userid);
        }
    }
}
