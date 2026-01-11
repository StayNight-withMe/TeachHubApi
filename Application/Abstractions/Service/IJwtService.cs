using Core.Models.TargetDTO.Users.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Service
{
    public interface IJwtService
    {

        public string GenerateJwt(UserAuthDto user);
        //public ClaimsPrincipal ValidateToken(string token);
        public string? RefreshToken(string jwttoken);

    }
}
