namespace infrastructure.Entitiеs
{
    public class UserRoleEntities
    {
        public int userid { get; set; }
        public int roleid { get; set; }
        public UserEntities user { get; set; }
    }
}
