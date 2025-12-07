using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.LessonFile.input
{
    public class MetaDataDTO
    {
        public int lessonid { get; set; }
        public string fileurl { get; set; } = string.Empty;
        public string name { get; set; }
        public string filetype { get; set; }
        public int order {  get; set; }
        public bool cloudstore { get; set; } = true;
    }
}
