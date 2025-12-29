using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ardalis.Specification;
using System.Threading.Tasks;

namespace Core.Specification.CourseSpecification
{
    public class ExistsCourseSpecification : Specification<CourseEntity, bool>
    {
        public ExistsCourseSpecification(int id, string courseName)
        {

            Query.AsNoTracking()
                .Where(c => c.creatorid == id &&
                       c.name == courseName);


        }
    }
}
