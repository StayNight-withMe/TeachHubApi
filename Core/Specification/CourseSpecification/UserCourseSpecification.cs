using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;

namespace Core.Specification.CourseSpecification
{
    public class UserCourseSpecification : Specification<CourseEntity>
    {
        public UserCourseSpecification(int userid, int courseid = default) 
        {

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
