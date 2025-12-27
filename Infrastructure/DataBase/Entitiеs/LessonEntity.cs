using System.ComponentModel.DataAnnotations.Schema;

namespace infrastructure.DataBase.Entitiеs
{
    public class LessonEntity
    {
        public int id { get; set; }
        public string name { get; set; } 
        public int chapterid { get; set; }
        public int? order {  get; set; }
        public int courseid { get; set; }
        public bool isvisible { get; set; }

        [ForeignKey(nameof(chapterid))]
        public ChapterEntity chapter { get; set; }

        [ForeignKey(nameof(courseid))]
        public CourseEntity course { get; set; }
    }
}
