using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Review.input
{
    public class ReviewInputDTO
    {
        public int courseid { get; set; }
        public string content { get; set; }
        [Range(1, 10)]
        public int review { get; set; }
    }
}
