using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Entitiеs
{
    public class LessonfilesEntitits
    {
        public int id {  get; set; }
        public string filekey { get; set; }
        public string filetype { get; set; }
        public string fileurl { get; set; }
        public string storage_type { get; set; }
        public int order { get; set; }

    }
}
