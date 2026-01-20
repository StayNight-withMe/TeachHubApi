using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Entitiеs
{
    public class ChapterEntity
    {
        public int id {  get; set; }
        public string name { get; set; }
        public int courseid { get; set; }
        public int? order {  get; set; }
        [ForeignKey(nameof(courseid))]
        public CourseEntity course { get; set; }

    }
}
