using Core.Model.BaseModel.Course;
using infrastructure.Utils.HashIdConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Courses.output
{
    public class CourseOutputDTO : BaseCourse
    {
        public Hashid id { get; set; }
        public string? iconurl { get; set; }
        public string creatorname {  get; set; }
        public bool favorite { get; set; }
        public Hashid creatorid { get; set; }
        public decimal? rating { get; set; }
        public Dictionary<Hashid, string> categorynames { get; set; } 
        public DateTime createdat {  get; set; }
    }
}
