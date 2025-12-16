using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.Users.output
{
    public class checkEmailDTO
    {
       public bool available { get; set; }
       public string reasonCode { get; set; }
    }
}
