using infrastructure.DataBase.Entitiеs;

namespace infrastructure.Utils.Mapping.MapperDTO
{
    public class UserRoleMappingSource
    {
        public UserEntity User { get; set; }
        public int RoleId { get; set; }
    }
}
