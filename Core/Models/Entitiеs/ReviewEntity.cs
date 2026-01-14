using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Entitiеs
{
    public class ReviewEntity
    {
        public int id { get; set; }
        public int userid { get; set; }
        public int courseid { get; set; } 
        public string content { get; set; }
        public int review { get; set; }
        public DateTime createdat { get; set; }
        public DateTime lastchangedat { get; set; } 
        public int likecount { get; set; }
        public int dislikecount { get; set; }

        [ForeignKey(nameof(userid))]
        public UserEntity user { get; set; }

        [ForeignKey(nameof(courseid))]
        public UserEntity course { get; set; }
    }
}
