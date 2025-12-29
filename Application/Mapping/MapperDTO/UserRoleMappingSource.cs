using infrastructure.DataBase.Entitiеs;

namespace Application.Mapping.MapperDTO
{
    public class UserRoleMappingSource
    {
        public UserEntity User { get; set; }
        public int RoleId { get; set; }
    }
}
