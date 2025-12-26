using Core.Model.BaseModel.Lesson;
using infrastructure.Utils.HashIdConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Lesson.output
{
    public class lessonOutputDTO : BaseLesson
    {
        public Hashid id { get; set; }
    }
}
