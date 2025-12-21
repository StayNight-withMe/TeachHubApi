using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Review.input
{
    public class ReviewChangedDTO
    {
        public int reviewid { get; set; }

        [Range(0, 10)]
        public int review {  get; set; }

        [MaxLength(255)]
        public string content { get; set; }
    }
}
