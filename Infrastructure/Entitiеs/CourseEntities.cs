using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace infrastructure.Entitiеs
{
    public class CourseEntities
    {
        public int id { get; set; }
        public string? imgfilekey { get; set; }
        public string name { get; set; } 
        public string description { get; set; } 
        public DateTime createdat {  get; set; } 
        public int? creatorid { get; set; }
        public decimal? rating { get; set; } 
        public string field {  get; set; }
        public NpgsqlTsVector searchvector { get; set; }

        [ForeignKey(nameof(creatorid))]
        public UserEntities user { get; set; }
    }
}
