using Core.Common.EnumS;
using infrastructure.Utils.HashIdConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Courses.input
{
    public class CourseSetImageDTO
    {
        public Hashid courseid { get; set; }
        public SetImageStatus setstatus { get; set; }
    }
}
