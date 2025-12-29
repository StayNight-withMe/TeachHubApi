using Asp.Versioning;
using AutoMapper;
using Core.Common.EnumS;
using Core.Interfaces.Utils;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Users.input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;
using System.Threading.Tasks;
using testApi.WebUtils.JwtClaimUtil;
using Core.Common.Types;
using Core.Common.Types.HashId;
using Application.Abstractions.Service;


namespace testApi.EndPoints
{
    [ApiController]
    [Route("api/users")]
    [Tags("Пользователи")]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _usersService;

        private readonly IJwtService _jwtService;

        private readonly IHeaderService _headerService;

        private readonly IOutputCacheStore _outputCacheStore;

        private readonly JwtClaimUtil _claims;

        public UserController( 
            IUsersService usersService,
            IJwtService jwtService,
            IHeaderService headerService, 
            IOutputCacheStore outputCacheStore,
            JwtClaimUtil claims
            )
        {
            _claims = claims;
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


        [HttpGet("exists")]
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


        [HttpDelete("admin/{id}/soft")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminAgressiveDelete(
            [FromRoute] Hashid id,
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


        /// <summary>
        /// Delete user by admin (soft delete)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpDelete("admin/{id}/hard")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminSoftDelete(
            [FromRoute] Hashid id,
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

        [HttpDelete("remove")]
        [Authorize]
        public async Task<IActionResult> SoftDelete(
            CancellationToken ct
            )
        {
            var result = await _usersService.SoftDelete(_claims.UserId);
            return await EntityResultExtensions.ToActionResult(result, this, 
                opt:  async () =>
            {
                await _outputCacheStore.EvictByTagAsync("check-email", ct);
            });
        }


    }
}
