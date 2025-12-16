using AutoMapper;
using Core.Common;
using Core.Interfaces.Service;
using Core.Interfaces.Utils;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Users.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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
        private readonly IOutputCacheStore _outputCacheStore;
        public UserController( IUsersService usersService,
            IJwtService jwtService,
            IHeaderService headerService, 
            IOutputCacheStore outputCacheStore
            )
        {
            _usersService = usersService;
            _jwtService = jwtService;
            _headerService = headerService;
            _outputCacheStore = outputCacheStore;
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            [FromBody]RegistrationUserDto registerUser,
            CancellationToken ct
            )
        {
        
            var result = await _usersService.RegistrationUser(
                registerUser, 
                PublicRole.user, 
                _headerService.GetIp(), 
                _headerService.GetUserAgent(),
                ct
                );

                if (!result.IsCompleted)
                {
                    await EntityResultExtensions.ToActionResult(result, this);
                }
            await _outputCacheStore.EvictByTagAsync("check-email", ct);
            return Ok(new { token = _jwtService.GenerateJwt(result.Value) });
        }


        [HttpGet("check-email")]
        [OutputCache(PolicyName = "CheckEmail1Hour")]
        public async Task<IActionResult> CheckEmail(
            [FromQuery] string email,
            CancellationToken ct
            )
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
        public async Task<IActionResult> AdminAgressiveDelete(
            int id,
            CancellationToken ct
            )
        {
           var result = await _usersService.AgressiveRemoveUser(id);
            return await EntityResultExtensions.ToActionResult(result, this,
          opt: async () =>
          {
              await _outputCacheStore.EvictByTagAsync("check-email", ct);
          });
        }

        [HttpDelete("admin/softremove/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminSoftDelete(
            int id,
            CancellationToken ct 
            )
        {
            var result = await _usersService.AgressiveRemoveUser(id);
            return await EntityResultExtensions.ToActionResult(result, this,
         opt: async () =>
         {
             await _outputCacheStore.EvictByTagAsync("check-email", ct);
         });

        }

        [HttpPatch("remove")]
        [Authorize]
        public async Task<IActionResult> SoftDelete(
            CancellationToken ct
            )
        {
            var result = await _usersService.SoftDelete(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return await EntityResultExtensions.ToActionResult(result, this, 
                opt:  async () =>
            {
                await _outputCacheStore.EvictByTagAsync("check-email", ct);
            });
        }


    }
}
