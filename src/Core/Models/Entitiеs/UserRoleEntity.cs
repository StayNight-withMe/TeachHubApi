namespace Core.Models.Entitiеs
{
    public class UserRoleEntity
    {
        public int userid { get; set; }
        public int roleid { get; set; }
        public UserEntity user { get; set; }
    }
}
