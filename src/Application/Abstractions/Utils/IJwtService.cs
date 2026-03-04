using Core.Models.TargetDTO.Users.input;

namespace Application.Abstractions.Utils
{
    public interface IJwtService
    {

        public string GenerateJwt(UserAuthDto user);
        //public ClaimsPrincipal ValidateToken(string token);
        public string? RefreshToken(string jwttoken);

    }
}
