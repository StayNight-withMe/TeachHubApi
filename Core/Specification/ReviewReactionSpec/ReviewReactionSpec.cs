using Ardalis.Specification;
using Core.Models.Entitiеs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specification.ReviewReactionSpec
{
    public class ReviewReactionSpec : Specification<ReviewreactionEntity>, ISingleResultSpecification
    {
        public ReviewReactionSpec(int reviewId, int userId)
        {
            Query.Where(r => r.reviewid == reviewId && 
                        r.userid == userId);
        }
    }
}
