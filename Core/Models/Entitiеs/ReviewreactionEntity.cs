using Core.Common.EnumS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Entitiеs
{
    public class ReviewreactionEntity
    {
        public int id { get; set; }
        public int reviewid { get; set; }
        public int userid { get; set; }

        [Column("reactiontype", TypeName = "reaction_type")]
        public reaction_type reactiontype { get; set; }

        [ForeignKey(nameof(reviewid))]
        public ReviewEntity review { get; set; }

        [ForeignKey(nameof(userid))]
        public UserEntity user { get; set; }
    }
}
