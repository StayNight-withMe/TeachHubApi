using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.BaseModel.Chapter;

namespace Core.Model.TargetDTO.Chapter.input
{
    public class ChapterUpdateDTO
    {
        public int id { get; set; }
        public string? name { get; set; }
        public int? order { get; set; }
    }
}
