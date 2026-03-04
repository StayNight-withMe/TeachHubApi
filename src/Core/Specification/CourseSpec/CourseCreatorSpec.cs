using Ardalis.Specification;
using Core.Models.Entitiеs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specification.CourseSpec
{
    public class CourseCreatorSpec : Specification<CourseEntity>
    {
        public CourseCreatorSpec(int courseId, int userId)
        {
            Query.Where(c => c.id == courseId && c.creatorid == userId)
                 .AsNoTracking();
        }
    }
}
