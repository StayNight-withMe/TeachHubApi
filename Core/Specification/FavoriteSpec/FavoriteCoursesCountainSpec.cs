using Ardalis.Specification;
using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specification.FavoriteSpec
{
    public class FavoriteCoursesCountainSpec : Specification<FavoritEntity>
    {
        public FavoriteCoursesCountainSpec(int userId, List<int> courseIds)
        {
            Query.AsNoTracking()
                 .Where(f => f.userid == userId && 
                 courseIds.Contains(f.courseid));
        }
    }
}
