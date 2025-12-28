using Core.Model.TargetDTO.Courses.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.Mapping.MapperDTO
{
    public class CourcesMappingSource
    {
        public CreateCourseDTO CourceDTO { get; set; }
        public int id { get; set; }
        public string field { get; set; }
    }
}
