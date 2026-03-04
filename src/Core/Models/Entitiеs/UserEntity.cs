namespace Core.Models.Entitiеs
{
    public class UserEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; } 
        public string email { get; set; }
        public bool isdelete { get; set; }
    }
}
