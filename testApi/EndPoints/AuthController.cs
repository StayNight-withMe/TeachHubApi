using Core.Interfaces.Service;
using Core.Interfaces.Utils;
using Core.Model.TargetDTO.Auth.input;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        private readonly IJwtService _jwtService;

        private readonly IHeaderService _headerService;

        public AuthController(IAuthService authService, IJwtService jwtService, IHeaderService headerService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _headerService = headerService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody]LoginUserDTO loginUserDTO)
        {
            var result = await _authService.LoginUser(loginUserDTO, _headerService.GetIp(), _headerService.GetUserAgent());
            if (result.IsCompleted)
            {
                var token = _jwtService.GenerateJwt(result.Value);
                return Ok(new { token });
            }
            else
            {
                return EntityResultExtensions.ToActionResult(result, this);
            }
            
        }

        [HttpPost("refrash")]
        public async Task<IActionResult> RefrashToken()
        {
            string? oldJwt = Request.Headers.Authorization;
            if (oldJwt == null)
            {
                return Unauthorized(new { message = "Токен отсутствует" });
            }
             var newToken = _jwtService.RefreshToken(GetToken(oldJwt));
            if(newToken != null)
            {
                return Ok(new { token = newToken } );
            }
            return Unauthorized(new { message = "Токен недействителен" });
        }


        private string GetToken(string token)
        {
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Длина токена: {token.Length}");
                Console.WriteLine($"Токен: {token["Bearer ".Length..].Trim()}");
                return token["Bearer ".Length..].Trim();

            }
            else
                Console.WriteLine("косяк передачи");
                return null;
        }


    }
}
