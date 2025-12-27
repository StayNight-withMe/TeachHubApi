using System.ComponentModel.DataAnnotations.Schema;

namespace infrastructure.DataBase.Entitiеs
{
    public class FavoritEntities
    {
        public int userid { get; set; }
        public int courseid { get; set; }
        public DateTime favoritdate { get; set; } = DateTime.Now.ToUniversalTime();

        [ForeignKey(nameof(courseid))]
        public CourseEntity course { get; set; }

        [ForeignKey(nameof(userid))]
        public UserEntity user { get; set; }
    }
}
