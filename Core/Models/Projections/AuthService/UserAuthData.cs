using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Projections.AuthService
{
    public record UserAuthData(int id, string password, string email, string name);
}
