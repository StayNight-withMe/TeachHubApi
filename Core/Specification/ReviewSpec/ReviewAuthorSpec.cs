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
    public class ReviewAuthorSpec : Specification<ReviewEntity>, ISingleResultSpecification
    {
        public ReviewAuthorSpec(int reviewId, int userId)
        {
            Query.Where(r => r.id == reviewId && r.userid == userId)
                 .AsNoTracking(); 
        }
    }
}
