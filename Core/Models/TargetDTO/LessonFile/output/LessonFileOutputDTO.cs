using Core.Common.Types.HashId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.LessonFile.output
{
    public class LessonFileOutputDTO
    {
        public Hashid id {  get; set; }
        public string filename { get; set; }
        public string url { get; set; }
        public int order {  get; set; }

    }
}
