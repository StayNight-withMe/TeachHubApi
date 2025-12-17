using System.ComponentModel.DataAnnotations.Schema;

namespace infrastructure.Entitiеs
{
    public class ReviewEntities
    {
        public int id { get; set; }
        public int userid { get; set; }
        public int courseid { get; set; } 
        public string content { get; set; }
        public int review { get; set; }
        public DateTime createdat { get; set; }
        public DateTime lastchangedat { get; set; }  = DateTime.Now;
        public int likecount { get; set; }
        public int dislikecount { get; set; }

        [ForeignKey(nameof(userid))]
        public UserEntities user { get; set; }

        [ForeignKey(nameof(courseid))]
        public UserEntities course { get; set; }
    }
}
