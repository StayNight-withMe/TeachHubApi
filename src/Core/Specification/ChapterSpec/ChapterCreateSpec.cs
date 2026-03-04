using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.ChapterSpec
{
    public class ChapterCreateSpec : Specification<ChapterEntity>
    {
        public ChapterCreateSpec(int order, int courseid, string name)
        {
            Query.AsNoTracking()
                 .Where(c => c.courseid == courseid &&
                        (c.order == order ||
                        c.name == name)
            );
        }
    }
}
