using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Courses.input
{
    public class CourseSetImageDTO
    {
        public string ContentType { get; set; }
        public int courseid { get; set; }
        public SetImageStatus SetStatus { get; set; }
    }
}
