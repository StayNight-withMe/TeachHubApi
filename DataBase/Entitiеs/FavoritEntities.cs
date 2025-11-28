using System.ComponentModel.DataAnnotations.Schema;

namespace infrastructure.Entitiеs
{
    public class FavoritEntities
    {
        public int userid { get; set; }
        public int courseid { get; set; }
        public DateTime favoritdate { get; set; } = DateTime.Now.ToUniversalTime();

        [ForeignKey(nameof(courseid))]
        public CourseEntities course { get; set; }

        [ForeignKey(nameof(userid))]
        public UserEntities user { get; set; }
    }
}
