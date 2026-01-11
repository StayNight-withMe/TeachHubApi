using Core.Common.Types.HashId;
using Core.Models.BaseModel.Chapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Chapter.input
{
    public class CreateChapterDTO : BaseChapter
    {
        public Hashid courseid { get; set; }
    }
}
