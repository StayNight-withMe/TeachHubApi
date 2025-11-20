using infrastructure.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.Mapping.MapperDTO
{
    public class ChapterJoinMappingSourse
    {
       public int creatorid { get; set; }
       public ChapterEntity chapter { get; set; }
    }
}
