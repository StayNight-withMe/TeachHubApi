using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Lesson.input
{
    public class createLessonDTO
    {
        public int order {  get; set; }
        public string name { get; set; }
        public int courseid { get; set; }
        public int chapterid { get; set; }
    }
}
