using Ardalis.Specification;
using Core.Models.Entitiеs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specification.LessonSpec
{
    /// <summary>
    /// Спецификация для проверки: принадлежит ли урок пользователю (через создателя курса)
    /// </summary>
    public class LessonOwnerSpec : Specification<LessonEntity, int>
    {
        public LessonOwnerSpec(int lessonId, int userId)
        {
            Query.AsNoTracking()
                 .Where(l => l.id == lessonId && 
                 l.course.creatorid == userId)
                 .Select(l => l.id); 
        }
    }
}
