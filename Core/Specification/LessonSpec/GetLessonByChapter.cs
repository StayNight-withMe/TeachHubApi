using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.LessonSpec
{
    public class GetLessonByChapter : Specification<LessonEntity>
    {
        public GetLessonByChapter(int chapterid, bool isvisible = true, int userid = 0)
        {
            Query
                .AsNoTracking()
                .Where(c => c.chapterid == chapterid &&
                       c.isvisible == isvisible);
        }
    }
}
