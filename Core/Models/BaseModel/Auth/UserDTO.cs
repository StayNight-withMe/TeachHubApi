using Core.Common.Types.HashId;
using HashidsNet;

namespace Core.Models.BaseModel.Auth
{
    public class UserDto
    {
        public Hashid id { get; set; }
        public string name { get; set; }
        public string password { get; set; } 
        public string email { get; set; }
    }
}
