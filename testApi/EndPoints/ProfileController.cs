using Application.Abstractions.Service;
using Asp.Versioning;
using Core.Common.Types.HashId;
using Core.Models.TargetDTO.Profile.common;
using Core.Models.TargetDTO.Profile.input;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using testApi.WebUtils.EntityResultExtensions;
using testApi.WebUtils.JwtClaimUtil;

namespace testApi.EndPoints
{
    
    [ApiController]
    [Route("api/profile")]
    [Tags("Профили")]
    [ApiVersion("1.0")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        private readonly JwtClaimUtil _jwtClaimUtil;

        public ProfileController(
            IProfileService profileService,
            JwtClaimUtil jwtClaimUtil
            ) 
        {
            _profileService = profileService;
            _jwtClaimUtil = jwtClaimUtil;
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateProfile(
            [FromBody]ChangeProfileDTO dto,
            CancellationToken ct
            )
        {
            var result = await _profileService.ChangeProfile(
                dto, 
                _jwtClaimUtil.UserId, 
                ct);

            return await EntityResultExtensions.ToActionResult(result, this);
        }

        [RequestSizeLimit(11 * 1024 * 1024)]
        [HttpPut("icon")]
        public async Task<IActionResult> ChangeProfileIcon(
            IFormFile? file,
            [FromForm]ProfileSetImageDTO dto,
            CancellationToken ct
            )
        {
            var fileStrem = file.OpenReadStream();
                
             if(fileStrem == null)
            {
                return BadRequest();
            }

            var result = await _profileService.ChangeProfileIcon(
                fileStrem,
                _jwtClaimUtil.UserId, 
                dto, 
                file.ContentType, 
                ct);

            return await EntityResultExtensions.ToActionResult(result, this);
        }

        [OutputCache(PolicyName = "10min")]
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetUserProfile(
            [FromRoute]Hashid userid, 
            CancellationToken ct)
        {
            var resukt = await _profileService.GetUserProfile(userid.Value, ct);

            return await EntityResultExtensions.ToActionResult(resukt, this);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfile(
            CancellationToken ct)
        {
            var resukt = await _profileService.GetUserProfile(_jwtClaimUtil.UserId, ct);

            return await EntityResultExtensions.ToActionResult(resukt, this);
        }
    }
}
