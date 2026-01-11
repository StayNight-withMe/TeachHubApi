using Core.Common.Types.HashId;
using Core.Model.BaseModel.Lesson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Lesson.input
{
    public class LessonUpdateDTO
    {
        public Hashid id { get; set; }
        public string? name { get; set; }
        public int? order { get; set; }
    }
}
