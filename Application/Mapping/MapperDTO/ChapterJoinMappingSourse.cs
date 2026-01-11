using Core.Models.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping.MapperDTO
{
    public class ChapterJoinMappingSourse
    {
       public int creatorid { get; set; }
       public ChapterEntity chapter { get; set; }
    }
}
