using Core.Model.BaseModel.Chapter;
using infrastructure.Utils.HashIdConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Chapter.output
{
    public class ChapterOutDTO : BaseChapter
    {
        public Hashid id { get; set; }
    }
}
