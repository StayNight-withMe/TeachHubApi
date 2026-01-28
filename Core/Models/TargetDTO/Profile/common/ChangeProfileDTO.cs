using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Profile.common
{
    public class ChangeProfileDTO
    {
        public string bio { get; set; }
        public Dictionary<string, string> sociallinks { get; set; }
    }
}
