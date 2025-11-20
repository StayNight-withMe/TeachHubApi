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
        public int id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public string? field { get; set; }
    }
}

