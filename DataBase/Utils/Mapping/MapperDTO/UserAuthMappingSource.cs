using infrastructure.Entitiеs;

namespace infrastructure.Utils.Mapping.MapperDTO
{
    public class UserAuthMappingSource
    {
        public UserEntities user { get; set; }
        public string role { get; set; }
        public string ip {  get; set; }
        public string UserAgent { get; set; }
    }
}
