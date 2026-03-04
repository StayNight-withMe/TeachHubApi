using Core.Specification.AuthSpec;


namespace Application.Mapping.MapperDTO
{
    public class UserAuthMappingSource
    {
        public int id { get; set; }
        public UserAuthData user { get; set; }
        public string role { get; set; }
        public string ip {  get; set; }
        public string UserAgent { get; set; }
    }
}
