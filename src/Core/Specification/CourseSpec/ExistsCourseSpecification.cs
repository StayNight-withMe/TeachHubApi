using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ardalis.Specification;
using System.Threading.Tasks;
using Core.Models.Entitiеs;

namespace Core.Specification.CourseSpec
{
    public class ExistsCourseSpecification : Specification<CourseEntity>
    {
        public ExistsCourseSpecification(int id, string courseName)
        {

            Query.AsNoTracking()
                .Where(c => c.creatorid == id &&
                       c.name == courseName);


        }
    }
}
