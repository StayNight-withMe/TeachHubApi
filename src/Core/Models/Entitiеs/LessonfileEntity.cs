using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Entitiеs
{
    public class LessonfileEntity
    {
        public int id {  get; set; }
        public int lessonid { get; set; }
        public string filekey { get; set; }
        public string filename { get; set; }
        public string filetype { get; set; }
        public bool cloudstore { get; set; }
        public int order { get; set; }

        [ForeignKey(nameof(lessonid))]
        public LessonEntity lesson { get; set; }
    }
}
