using Ardalis.Specification;
using Core.Models.Entitiеs;

namespace Core.Specification.AuthSpec
{
    public record UserAuthData(int id, string password, string email, string name);
    public class UserAuthSpec : Specification<UserEntity, UserAuthData>
    {
        public string Email { get; set; }
        public UserAuthSpec(string email, bool isdelete = false) 
        {
            Email = email;
            Query
                .AsNoTracking()
                .Where(c => c.email == email &&
                        c.isdelete == isdelete)
                .Select(c => new UserAuthData(c.id, c.password, c.email, c.name));
        }

    }
}
