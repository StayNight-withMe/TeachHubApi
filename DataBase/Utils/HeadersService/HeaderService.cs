using Core.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.HeadersService
{
    public class HeaderService : IHeaderService
    {
        private readonly IHttpContextAccessor _httpContext;

        public HeaderService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetIp()
        {
            try
            {
                string xff = _httpContext.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? string.Empty;

                if (xff != null)
                {
                    var ip = xff.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if(ip.Length > 0)
                    {
                        return ip[0];
                    }
                  
                    else
                    {
                        return string.Empty;
                    }
                        
                }

                return _httpContext.HttpContext?.Connection.RemoteIpAddress.ToString() ?? string.Empty;
            }
           catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} ошибка в headerService");
                return string.Empty;
            }

        }

        public string GetUserAgent() => _httpContext.HttpContext?.Request.Headers["User-Agent"].ToString() ?? string.Empty;


    }
}
