using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Entitiеs
{
    public class Course_CategoriesEntities
    {
        public int courseid { get; set; }
        public int categoryid { get; set; }

        [ForeignKey(nameof(categoryid))]
        public CategoriesEntities categories { get; set; }

        [ForeignKey(nameof(courseid))]
        public CourseEntities course { get; set; }
    }
}
