using Asp.Versioning;
using Core.Interfaces.Utils;
using Core.Model.TargetDTO.Courses.output;
using infrastructure.Utils.HashIdConverter;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Core.Common.Types.HashId;
using Application.Abstractions.Service;
using Core.Models.TargetDTO.Auth.input;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/auth")]
    [Tags("Аутентификация")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        private readonly IJwtService _jwtService;

        private readonly IHeaderService _headerService;

        public AuthController(
            IAuthService authService, 
            IJwtService jwtService, 
            IHeaderService headerService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _headerService = headerService;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            var id = new Hashid(123);
            // Это заставит сериализатор работать прямо здесь, в коде
            var json = System.Text.Json.JsonSerializer.Serialize(id, new JsonSerializerOptions
            {
                Converters = { new HashidJsonConverter() }
            });

            return Ok(new
            {
                DirectJson = json,
                RawObject = id
            }); 
        }



        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody]LoginUserDTO loginUserDTO)
        {
            var result = await _authService.LoginUser(
                loginUserDTO, 
                _headerService.GetIp(), 
                _headerService.GetUserAgent());
            if (result.IsCompleted)
            {
                var token = _jwtService.GenerateJwt(result.Value);
                return Ok(new { token });
            }
            else
            {
                return await EntityResultExtensions.ToActionResult(result, this);
            }
        }

        [HttpPost("refrash")]
        public IActionResult RefrashToken()
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
                return token["Bearer ".Length..].Trim();
            }
            else
                Console.WriteLine("косяк передачи");
                return null;
        }


    }
}
