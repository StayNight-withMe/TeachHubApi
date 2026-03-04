using Core.Common.Types.HashId;
using System.Security.Claims;

namespace testApi.WebUtils.JwtClaimUtil
{
    public class JwtClaimUtil
    {
        private readonly IHttpContextAccessor _context;

        public int UserId { get => Convert.ToInt32(GetClaim(ClaimTypes.NameIdentifier, true)); }
        public string? Email { get => GetClaim(ClaimTypes.Email); }
        public string? Ip { get => GetClaim("ip"); }
        public string? Role { get => GetClaim(ClaimTypes.Role); }
        public string? Name { get => GetClaim(ClaimTypes.Role); }
        public string? Exp { get => GetClaim("exp"); }
        public string? UA { get => GetClaim("user-agent"); }

        private readonly bool _isAuthenticated;

        public JwtClaimUtil(IHttpContextAccessor httpContext)
        {
            _context = httpContext;
            _isAuthenticated = _context.HttpContext.User.Identity.IsAuthenticated;
        }

        private string? GetClaim(string claimNamr, bool decode = false)
        {
            if (_isAuthenticated)
            {
                string claim = _context.HttpContext.User.FindFirstValue(claimNamr).ToString();

                if(decode)
                {
                    claim = HashidsHelper.Decode(claim).ToString();
                }
                return claim;
            }
            return null;
        }

    }
}
