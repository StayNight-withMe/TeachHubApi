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
    public class LessonFilesByLessonSpec : Specification<LessonfileEntity>
    {
        public LessonFilesByLessonSpec(int lessonId)
        {
            Query.Where(f => f.lessonid == lessonId)
                 .AsNoTracking();
        }
    }
}
