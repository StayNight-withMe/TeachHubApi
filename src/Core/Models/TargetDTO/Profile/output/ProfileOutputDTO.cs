using Core.Common.Types.HashId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Profile.output
{
    public class ProfileOutputDTO
    {
        public Hashid userid { get; set;}
        public string iconurl { get; set;}
        public string name { get; set; }
        public string bio { get; set; }
        public Dictionary<string, string> sociallinks { get; set; }
    }
}
