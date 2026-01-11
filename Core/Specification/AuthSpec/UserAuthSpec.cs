using Ardalis.Specification;
using Core.Model.TargetDTO.Auth.input;
using Core.Models.Entitiеs;
using Core.Models.Projections.AuthService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Specification.AuthSpec
{
    public class UserAuthSpec : Specification<UserEntity, UserAuthData>
    {
        public UserAuthSpec(string email, bool isdelete = false) 
        {
            Query
                .AsNoTracking()
                .Where(c => c.email == email &&
                        c.isdelete == isdelete)
                .Select(c => new UserAuthData(c.id, c.password, c.email, c.name));


        }

    }
}
