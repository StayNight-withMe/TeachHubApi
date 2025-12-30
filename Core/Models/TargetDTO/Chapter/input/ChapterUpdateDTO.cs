using Core.Common.Types.HashId;
using Core.Model.BaseModel.Chapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Chapter.input
{
    public class ChapterUpdateDTO
    {
        public Hashid id { get; set; }
        public string? name { get; set; }
        public int? order { get; set; }
    }
}
