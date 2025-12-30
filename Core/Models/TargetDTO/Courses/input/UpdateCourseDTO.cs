using Core.Common.Types.HashId;
using Core.Model.BaseModel.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Courses.input
{
    public class UpdateCourseDTO 
    {
        public Hashid id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public Hashid? field { get; set; }
        public int[]? categoryid { get; set; }
    }
}

