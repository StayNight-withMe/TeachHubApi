using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.CourseSpec
{
    public class UserCourseSpecification : Specification<CourseEntity>
    {
        public UserCourseSpecification(int userid, int courseid = default, bool tracking = false) 
        {
            if(!tracking)
            {
                Query.AsNoTracking();
            }
            if (courseid != default)
            {
                Query.Where(c => c.id == courseid &&
                      c.creatorid == userid);
            }
            else
            {
                Query.Where(c => c.id == userid);
            }

            
          
        }    
    }
}
