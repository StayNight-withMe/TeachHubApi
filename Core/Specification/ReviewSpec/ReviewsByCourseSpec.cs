using Ardalis.Specification;
using Core.Models.Entitiеs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specification.ReviewSpec
{
    public class ReviewsByCourseSpec : Specification<ReviewEntity>
    {
        public ReviewsByCourseSpec(int courseId)
        {
            Query.Where(r => r.courseid == courseId)
                 .AsNoTracking();
        }
    }
}
