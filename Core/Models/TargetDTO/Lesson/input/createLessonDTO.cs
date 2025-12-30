using Core.Common.Types.HashId;
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
        public Hashid courseid { get; set; }
        public Hashid chapterid { get; set; }
    }
}
