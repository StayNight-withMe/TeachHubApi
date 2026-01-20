using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Entitiеs
{
    public class PostgresTsVector { }
    public class CourseEntity
    {
        public int id { get; set; }
        public string? imgfilekey { get; set; }
        public string name { get; set; } 
        public string description { get; set; } 
        public DateTime createdat {  get; set; } 
        public int? creatorid { get; set; }
        public decimal? rating { get; set; } 
        public string field {  get; set; }
        public PostgresTsVector searchvector { get; set; }

        [ForeignKey(nameof(creatorid))]
        public UserEntity user { get; set; }
    }
}
