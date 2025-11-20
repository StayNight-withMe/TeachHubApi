using Core.Model.BaseModel.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Courses.output
{
    public class CourseOutputDTO : BaseCourse
    {
        public string id { get; set; }
        public string creatorname {  get; set; }
        public DateTime createdat {  get; set; }
    }
}
