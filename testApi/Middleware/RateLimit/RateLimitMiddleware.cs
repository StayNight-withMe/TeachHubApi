using Core.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading.RateLimiting;

namespace testApi.Middleware.RateLimit
{
    public class RateLimitMiddleware
    {

        private readonly TimeSpan _limitTimeSpan = TimeSpan.FromSeconds(60);

        private readonly RequestDelegate _next;

        private readonly ILogger<RateLimitMiddleware> _logger;

        private ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _reqUests = new();

        private readonly int _limit = 10;



        public RateLimitMiddleware(RequestDelegate requestDelegate, ILogger<RateLimitMiddleware> logger, int secund, int requsestsCount)
        {
            _next = requestDelegate;
            _logger = logger;
            _limit = requsestsCount;
            _limitTimeSpan = TimeSpan.FromSeconds(secund);
        }



        public async Task InvokeAsync(HttpContext context, IHeaderService headerService)
        {
            var ip = headerService.GetIp();

            var qw = _reqUests.GetOrAdd(ip, _ => new ConcurrentQueue<DateTime>());

            //for(int i = 0; i < _reqUests[ip].Count; i++) 
            //{
            //    if (_reqUests[ip].TryPeek(out var time) && (DateTime.UtcNow - time) > _limitTimeSpan)
            //    {
            //        _reqUests[ip].TryDequeue(out _);
            //    }
            //}


            while (qw.TryPeek(out var oldest) && DateTime.UtcNow - oldest > _limitTimeSpan)
            {
                qw.TryDequeue(out _);
            }
            //(c => DateTime.UtcNow - c > _limitTimeSpan);            
            _logger.LogDebug($"количество запросв :: {qw.Count}");

            if (qw.Count >= _limit)
            {

                _logger.LogDebug("Лимит запросов превышен");
                await RateLimitDropError(context, qw);
                return;
            }

            qw.Enqueue(DateTime.UtcNow);

            if (qw.Count == 0)
            {
                _reqUests.TryRemove(ip, out _);
            }

            try
            {
                await _next(context);
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogDebug("сработал токен отмены в rateLimit middleware");
            }


        }


        // 11.14
        // 11.13
        // 11.12
        // 11.11



        public async Task RateLimitDropError(HttpContext context, ConcurrentQueue<DateTime> times)
        {

            context.Response.StatusCode = 429;
            context.Response.ContentType = "application/json";


            DateTime TimeLimit;
            if (times.TryPeek(out DateTime gg))
            {
                TimeLimit = gg + _limitTimeSpan;
            }
            else
            {
                TimeLimit = DateTime.UtcNow;
            }

            var response = new
            {
                error = "RateLimitError",
                limit = _limit,
                limitTimeSpan = _limitTimeSpan,
                resetTime = TimeLimit,
            };

            await context.Response.WriteAsJsonAsync(response);
        }






    }




}