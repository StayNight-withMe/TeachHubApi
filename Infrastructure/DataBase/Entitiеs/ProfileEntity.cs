using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.DataBase.Entitiеs
{
    public class ProfileEntity
    {
        public int userid { get; set; }
        public string? bio { get; set; }
        public string? avatarkey { get; set; }
        public Dictionary<string, string> sociallinks { get; set; } = new();
        public UserEntity user { get; set; }
    }
}
