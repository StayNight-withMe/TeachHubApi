using HashidsNet;
using infrastructure.Utils.HashIdConverter;

namespace Core.Model.BaseModel.Auth
{
    public class UserDto
    {
        public Hashid id { get; set; }
        public string name { get; set; }
        public string password { get; set; } 
        public string email { get; set; }
    }
}
