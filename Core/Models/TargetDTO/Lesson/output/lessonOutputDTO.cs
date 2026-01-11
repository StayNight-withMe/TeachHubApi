using Core.Common.Types.HashId;
using Core.Models.BaseModel.Lesson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Lesson.output
{
    public class lessonOutputDTO : BaseLesson
    {
        public Hashid id { get; set; }
    }
}
