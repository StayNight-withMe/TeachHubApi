using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace testApi.EndPoints
{
    
    [ApiController]
    [Route("api/profile")]
    [Tags("Профили")]
    [ApiVersion("1.0")]
    public class ProfileController : ControllerBase
    {
        public Task<IActionResult> UpdateProfile()
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> ChangeProfileIcon()
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetUserProfile()
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetMyProfile()
        {
            throw new NotImplementedException();
        }
    }
}
