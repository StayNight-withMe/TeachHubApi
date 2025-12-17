using Core.Interfaces.Repository;
using Core.Model.Options;
using infrastructure.Entitiеs;
using infrastructure.Utils.BloomFilter.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace infrastructure.BackgroundService
{
    public class BloomRebuildService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly ILogger<BloomRebuildService> _logger;

        private readonly IEmailChecker _emailChecker;

        private readonly TimeOnly _targetTime;

        //private readonly IBaseRepository<UserEntities> _userRepository;

        private IServiceProvider _serviceProvider;

        public BloomRebuildService(
            ILogger<BloomRebuildService> logger,
            //IBaseRepository<UserEntities> repository,
            IOptions<BloomRebuildOptions> options,
            IEmailChecker emailChecker,
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            //_userRepository = repository;
            _emailChecker = emailChecker;
            _targetTime = options.Value.RebuildTime;
        }


        protected async override Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("BloomRebuildService is starting.");
            while (!ct.IsCancellationRequested)
            {

                var now = DateTimeOffset.Now;
                var today = now.Date + _targetTime.ToTimeSpan();
                var nextTarget = today > now ? today : today.AddDays(1);
                var delay = nextTarget - now;

                try
                {
                    using(var scope =  _serviceProvider.CreateAsyncScope())
                    {
                        var _userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<UserEntities>>();
                        await _emailChecker.RebuildFilter(_userRepository, ct);
                    }
                   
                    _logger.LogInformation("BloomRebuildService started at: {time}", DateTimeOffset.Now);
                    await Task.Delay(delay);
                    _logger.LogInformation("BloomRebuildService finished at: {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in BloomRebuildService at: {time}", DateTimeOffset.Now);
                }

            }
        }
    }
}
