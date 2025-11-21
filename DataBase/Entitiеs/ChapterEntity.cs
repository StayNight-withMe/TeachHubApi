using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Entitiеs
{
    public class ChapterEntity
    {
        public int id {  get; set; }
        public string name { get; set; }
        public int courseid { get; set; }
        public int? order {  get; set; }
        [ForeignKey(nameof(courseid))]
        public CourseEntities course { get; set; }

    }
}
