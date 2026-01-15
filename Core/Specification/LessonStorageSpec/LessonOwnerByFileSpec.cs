using Ardalis.Specification;
using Core.Models.Entitiеs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specification.LessonStorageSpec
{
    public class LessonOwnerByFileSpec : Specification<LessonEntity>, ISingleResultSpecification
    {
        public LessonOwnerByFileSpec(int lessonId, int userId)
        {
            Query.Where(l => l.id == lessonId && l.course.creatorid == userId)
                 .Include(l => l.course) // Необходим для доступа к creatorid
                 .AsNoTracking();
        }
    }
}
