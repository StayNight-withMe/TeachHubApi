using Core.Common.Types.HashId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.LessonFile.input
{
    public class MetaDataLessonDTO
    {
        public Hashid lessonid { get; set; }
        public string fileurl { get; set; } = string.Empty;
        public string name { get; set; }
        public string filetype { get; set; }
        public int order {  get; set; }
        public bool cloudstore { get; set; } = true;
    }
}
