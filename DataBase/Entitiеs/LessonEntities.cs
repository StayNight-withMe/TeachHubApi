using System.ComponentModel.DataAnnotations.Schema;

namespace infrastructure.Entitiеs
{
    public class LessonEntities
    {
        public int id { get; set; }
        public string name { get; set; } 
        public int chapterid { get; set; }
        public int order {  get; set; }
        public int courseid { get; set; }
        public bool isVisible { get; set; }

        [ForeignKey(nameof(chapterid))]
        public ChapterEntity chapter { get; set; }

        [ForeignKey(nameof(courseid))]
        public CourseEntities course { get; set; }
    }
}
