using infrastructure.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.Mapping.MapperDTO
{
    public class ChapterMappingSource
    {
       public int courseid { get; set; }

        public ChapterEntity Chapter { get; set; }
    }
}
