using Amazon.Runtime;
using Amazon.S3;
using Application.Abstractions.Utils;
using Core.Models.Options;
using infrastructure.BackgroundService;
using infrastructure.Utils.BloomFilter.implementation;
using Microsoft.Extensions.Options;

namespace testApi.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<BloomOptions>(config.GetSection("Bloom"));
            services.Configure<BackblazeOptions>(config.GetSection("B2"));

            services.AddSingleton<IAmazonS3>(sp => {
                var opt = sp.GetRequiredService<IOptions<BackblazeOptions>>().Value;
                return new AmazonS3Client(
                    new BasicAWSCredentials(opt.KeyId, opt.ApplicationKey),
                    new AmazonS3Config { ServiceURL = opt.Endpoint, ForcePathStyle = true });
            });

            services.AddHostedService<BloomRebuildService>();
            services.AddSingleton<IEmailChecker, EmailChecker>();

            return services;
        }
    }
}
