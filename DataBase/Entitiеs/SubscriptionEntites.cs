using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Entitiеs
{
    public class SubscriptionEntites
    {
        public int followerid {  get; set; }
        public int followingid { get; set; }
        public DateTime followingdate { get; set; }

        [ForeignKey(nameof(followerid))]
        public UserEntities follower { get; set; }

        [ForeignKey(nameof(followingid))]
        public UserEntities following { get; set; }
    }
}
