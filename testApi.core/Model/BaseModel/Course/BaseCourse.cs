using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.BaseModel.Course
{
    public class BaseCourse
    {
            public string name { get; set; }
            public string description { get; set; }
            public string field { get; set; }
    }
}
