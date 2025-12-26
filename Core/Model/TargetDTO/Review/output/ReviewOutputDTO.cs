using infrastructure.Utils.HashIdConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Review.output
{
    public class ReviewOutputDTO
    {
        public Hashid id { get; set; }
        public Hashid userId { get; set; }
        public Hashid courseId { get; set; }
        public string content { get; set; }
        public int review { get; set; }
        public int likecount { get; set; }
        public DateTime lastchangedat { get; set; }
        public int dislikecount { get; set; }
        public DateTime createdat { get; set; }
    }
}
