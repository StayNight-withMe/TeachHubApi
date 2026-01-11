using Core.Common.Types.HashId;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Review.input
{
    public class ReviewICreateDTO
    {
        public Hashid courseid { get; set; }
        public string content { get; set; }
        [Range(1, 10)]
        public int review { get; set; }
    }
}
