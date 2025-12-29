using Ardalis.Specification;
using Core.Model.TargetDTO.Auth.input;
using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Specification.AuthSpec
{
    public class UserSpec : Specification<UserEntity>
    {
        public UserSpec(string email, bool isdelete = false) 
        {
            Query.Where(c => c.email == email &&
                        c.isdelete == false);


        }

    }
}
