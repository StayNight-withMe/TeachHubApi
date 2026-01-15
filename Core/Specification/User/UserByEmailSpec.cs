using Ardalis.Specification;
using Core.Models.Entitiеs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
