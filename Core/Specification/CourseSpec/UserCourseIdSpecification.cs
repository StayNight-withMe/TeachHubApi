using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.CourseSpec
{
    public class UserCourseIdSpecification : Specification<CourseEntity, int>
    {
        public UserCourseIdSpecification(int userid, int courseid = default)
        {
            Query.AsNoTracking()
                .Select(c => c.id);

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
