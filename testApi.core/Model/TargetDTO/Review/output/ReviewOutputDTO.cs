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
        public int id { get; set; }
        public int userId { get; set; }
        public int courseId { get; set; }
        public string content { get; set; }
        public int likeCount { get; set; }
        public int dislikeCount { get; set; }
        public TimeSpan createdat { get; set; }
    }
}
