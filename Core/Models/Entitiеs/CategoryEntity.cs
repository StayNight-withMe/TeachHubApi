using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.DataBase.Entitiеs
{
    public class CategoryEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public int parentid { get; set; }

        [ForeignKey(nameof(parentid))]
        public CategoryEntity categories { get; set; }
    }
}
