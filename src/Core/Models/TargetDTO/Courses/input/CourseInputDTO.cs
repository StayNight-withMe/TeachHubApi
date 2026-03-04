using Core.Common.Types.HashId;
using Core.Models.BaseModel.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Courses.input
{
    public class CourseInputDTO : BaseCourse
    {
        public string? creatorName { get; set; }
        public DateTime createdat { get; set; }
        public Hashid[] categoryid { get; set; }

    }
}
