using AutoMapper;
using Core.Common;
using Core.Interfaces.Service;
using Core.Interfaces.Utils;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Users.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IJwtService _jwtService;
        private readonly IHeaderService _headerService;

        public UserController(IUsersService usersService, IJwtService jwtService, IHeaderService headerService)
        {
            _usersService = usersService;
            _jwtService = jwtService;
            _headerService = headerService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegistrationUserDto registerUser)
        {
        
            var result = await _usersService.RegistrationUser(registerUser, PublicRole.user, _headerService.GetIp(), _headerService.GetUserAgent());

                if (!result.IsCompleted)
                {
                    return BadRequest(new { error = $"{result.MessageForUser}" });
                }

            return Ok(new { token = _jwtService.GenerateJwt(result.Value) });
        }

        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail([FromQuery] string email)
        {
           var result = await _usersService.CheckEmail(email);
            if(result.IsCompleted == true)
            {
                return Ok(result.Value);
            }
            else
            {
                return StatusCode(500, "ошибка проверки email");
            }
          
        }


        [HttpDelete("admin/hardremove/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminAgressiveDelete(int id)
        {
           var result = await _usersService.AgressiveRemoveUser(id);
           return EntityResultExtensions.ToActionResult(result, this);
        }

        [HttpDelete("admin/softremove/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminSoftDelete(int id)
        {
            var result = await _usersService.AgressiveRemoveUser(id);
            return EntityResultExtensions.ToActionResult(result, this);        

        }

        [HttpPatch("remove")]
        [Authorize]
        public async Task<IActionResult> SoftDelete()
        {
            var result = await _usersService.SoftDelete(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return EntityResultExtensions.ToActionResult(result, this);
        }


    }
}
