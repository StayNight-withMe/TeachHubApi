using System.ComponentModel.DataAnnotations.Schema;

namespace infrastructure.Entitiеs
{
    public class LessonEntities
    {
        public int id { get; set; }
        public string name { get; set; } 
        public int chapterid { get; set; }

        [ForeignKey(nameof(chapterid))]
        public ChapterEntity chapter { get; set; }
    }
}
