using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Entitiеs
{
    public class CategoriesEntities
    {
        public int id { get; set; }
        public string name { get; set; }
        public int parentid { get; set; }

        [ForeignKey(nameof(parentid))]
        public CategoriesEntities categories { get; set; }
    }
}
