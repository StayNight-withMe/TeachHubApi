using Core.Common.Types.HashId;
using Core.Models.BaseModel.Course;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Courses.input
{
    public class CreateCourseDTO : BaseCourse
    {
        [MaxLength(10, ErrorMessage = "category limit exceeded")]
        public Hashid[] categoryid { get; set; }

    }
}
