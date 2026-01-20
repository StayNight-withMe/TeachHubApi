using System.Net;
using System.Text.RegularExpressions;
using testApi.WebUtils.HeadersService.interfaces;

namespace testApi.Middleware.IpValidate
{
    public class IpValidateMidlleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IpValidateMidlleware> _logger;

        public IpValidateMidlleware(RequestDelegate requestDelegate, ILogger<IpValidateMidlleware> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, IHeaderService headerService)
        {
            try
            {
                var ip = headerService.GetIp();
          
                if(string.IsNullOrWhiteSpace(ip) || !IsValidIp(ip, out _))
                {
                    ResponseError(httpContext);
                    return;
                }
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    bool validateFirst = validateIp(ip, httpContext.User.FindFirst("ip")?.Value ?? string.Empty);

                    bool validateSecond = ValidateUserAgent(httpContext.User.FindFirst("User-Agent")?.Value ?? string.Empty, httpContext.Request.Headers.UserAgent.ToString());

                    if (!validateFirst && !validateSecond)
                    {
                        _logger.LogWarning($" ip-адрес в токене не совпадает с целевым ip-адрес");
                        ResponseError(httpContext);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ResponseError(httpContext);
                return;
            }
         

            try
            {
                await _next(httpContext);
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogDebug("сработал токен отмены в ip middleware");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "unckown");
            }

            
        }



        public void ResponseError(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
           httpContext.Response.WriteAsJsonAsync(new { ErrorMessage = "invalid ip" });
        }


        public bool IsValidIp(string ipString, out IPAddress? validatedIp)
        {
            validatedIp = null;
            if (string.IsNullOrWhiteSpace(ipString))
                return false;

          
            ipString = ipString.Trim();

      
            return IPAddress.TryParse(ipString, out validatedIp);
        }

        public bool validateIp(string ip1, string ip2)
        {
            
            var bytes1 = IPAddress.Parse(ip1).GetAddressBytes() ;
            var bytes2 = IPAddress.Parse(ip2).GetAddressBytes();


            return bytes1[0].Equals(bytes2[0])
                && bytes1[1].Equals(bytes2[1])
                && bytes1[2].Equals(bytes2[2]);

        }

        public bool ValidateUserAgent(string userAgent1, string userAgent2)
        {
            if (string.IsNullOrEmpty(userAgent1) || string.IsNullOrEmpty(userAgent2)) return false;

            // Заменяем ВСЕ цифры и точки на звёздочки
            var cleanUA1  = Regex.Replace(userAgent1, @"[\d\.]+", "*");

            var cleanUA2 = Regex.Replace(userAgent2, @"[\d\.]+", "*");


            if(cleanUA1 == cleanUA2) return true;
            return false;

        }



    }
}
