using Core.Model.BaseModel.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Courses.input
{
    public class CourseInputDTO : BaseCourse
    {
        public string? creatorName { get; set; }
        public DateTime createdat { get; set; }

    }
}
