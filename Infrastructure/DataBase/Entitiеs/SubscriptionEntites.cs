using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.DataBase.Entitiеs
{
    public class SubscriptionEntites
    {
        public int followerid {  get; set; }
        public int followingid { get; set; }
        public DateTime followingdate { get; set; }

        [ForeignKey(nameof(followerid))]
        public UserEntity follower { get; set; }

        [ForeignKey(nameof(followingid))]
        public UserEntity following { get; set; }
    }
}
