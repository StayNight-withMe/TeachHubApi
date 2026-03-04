using Application.Abstractions.Utils;
using Core.Common.Types.HashId;
using Core.Models.TargetDTO.Users.input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace infrastructure.Utils.JwtService
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _conf;

        private readonly SymmetricSecurityKey _key;

        private readonly ILogger<IJwtService> _logger;

        public JwtService(IConfiguration conf, ILogger<IJwtService> logger)
        {
            _conf = conf;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt:Key"]));
            _logger = logger;
        }



        public string GenerateJwt(UserAuthDto user)
        {

            var encodedId = HashidsHelper.Encode(user.id);
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, encodedId),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role.ToString()),
                new Claim(ClaimTypes.Name, user.name.ToString()),
                new Claim("ip", user.ip),
                new Claim("user-agent", user.useragent),
            };

            var token = new JwtSecurityToken(

                claims: claims,
                signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256),
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_conf["Jwt:Time"])),
                audience: _conf["Jwt:Audience"],
                issuer: _conf["Jwt:Issuer"]
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private DateTime? GetExpFromPrincipal(ClaimsPrincipal principal)
        {
            var expClaim = principal.FindFirst("exp");
            if (expClaim?.Value != null && long.TryParse(expClaim.Value, out long expUnix))
            {
                return DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            }
            return null; 
        }

        public string? RefreshToken(string jwttoken)
        {
            try 
            {

                if (jwttoken == null)
                {
                    return null;
                }

                ClaimsPrincipal ClaimsPprincipal = ValidateToken(jwttoken);

                if (ClaimsPprincipal == null)
                {
                    return null;
                }


                DateTime tokenTime = (DateTime)GetExpFromPrincipal(ClaimsPprincipal);

                if (tokenTime < DateTime.UtcNow.AddMinutes(-15) || tokenTime > DateTime.UtcNow)
                {
                    _logger.LogDebug("ошибка во времени в jwt-токене");
                    return null;
                }


                var token = new JwtSecurityToken(

                    claims: ClaimsPprincipal.Claims,
                    signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256),
                    expires: tokenTime.AddMinutes(30),
                    audience: _conf["Jwt:Audience"],
                    issuer: _conf["Jwt:Issuer"]
                    );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                return null;
            }

        }


        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validateParametr = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _conf["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _conf["Jwt:Audience"],
                ValidateLifetime = false,
                //RoleClaimType = ClaimTypes.Role
            };

            try
            {
                var principical = tokenHandler.ValidateToken(token, validateParametr, out _);
                return principical;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"токен {token} не прошел валидацию{ex.Message}");
                return null;
                
            }

                

        }



    }
}
