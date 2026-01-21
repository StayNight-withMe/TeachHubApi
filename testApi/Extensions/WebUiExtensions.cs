using Core.Common.Types.HashId;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using System.Text.Json.Serialization;
using testApi.WebUtils.HashIdConverter;

namespace testApi.Extensions;

public static class WebUiExtensions
{
    public static IServiceCollection AddWebUiServices(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new HashidModelBinderProvider());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new HashidJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressInferBindingSourcesForParameters = false;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.MapType<Hashid>(() => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("jR8vWd")
            });
        });

        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            o.ReportApiVersions = true;
        });

        services.AddOutputCache(opt =>
        {
            opt.AddPolicy("1min", poli => poli.Expire(TimeSpan.FromSeconds(60)).SetVaryByQuery("*").SetVaryByHeader("Authorization").Tag("1min"));
            opt.AddPolicy("10min", poli => poli.Expire(TimeSpan.FromMinutes(10)).SetVaryByQuery("*").SetVaryByHeader("Authorization").Tag("10min"));
            opt.AddPolicy("30min", poli => poli.Expire(TimeSpan.FromMinutes(30)).SetVaryByQuery("*").Tag("30min"));
            opt.AddPolicy("60min", poli => poli.Expire(TimeSpan.FromMinutes(60)).SetVaryByQuery("*").Tag("60min"));
            opt.AddPolicy("CheckEmail1Hour", c => c.Expire(TimeSpan.FromMinutes(60)).SetVaryByQuery("*").Tag("email-check"));
        });

        services.AddOpenApi();

        return services;
    }
}