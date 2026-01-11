using Core.Common.Types.HashId;
using Core.Models.BaseModel.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.Users.input
{
    public class UserAuthDto : UserDTO
    {
        public Hashid id { get; set; }  
        public string role { get; set; }
        public string ip { get; set; }
        public string useragent { get; set; }

    }
}
