using Core.Common.EnumS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Models.TargetDTO.Auth.input
{
    public class LoginUserDTO
    {
        public string email { get; set; }    
        public string password { get; set; }
        public AllRole role { get; set; }

    }
}
