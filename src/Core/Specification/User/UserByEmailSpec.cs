using Ardalis.Specification;
using Core.Models.Entitiеs;


namespace Core.Specification.User
{
    public class UserByEmailSpec : Specification<UserEntity>
    {
        public UserByEmailSpec(string email)
        {
            Query.Where(u => u.email == email)
                 .AsNoTracking();
        }
    }
}
