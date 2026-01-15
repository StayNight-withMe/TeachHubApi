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
    public class ReviewsByUserSpec : Specification<ReviewEntity>
    {
        public ReviewsByUserSpec(int userId)
        {
            Query.Where(r => r.userid == userId)
                 .AsNoTracking();
        }
    }
}
