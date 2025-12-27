using infrastructure.DataBase.Entitiеs;

namespace infrastructure.Utils.Mapping.MapperDTO
{
    public class UserAuthMappingSource
    {
        public int id { get; set; }
        public UserEntity user { get; set; }
        public string role { get; set; }
        public string ip {  get; set; }
        public string UserAgent { get; set; }
    }
}
