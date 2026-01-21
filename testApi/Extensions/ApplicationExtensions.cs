using Amazon.Runtime;
using Amazon.S3;
using Application.Abstractions.Service;
using Application.Abstractions.Utils;
using Application.Mapping.AutoMapperProfiles;
using Application.Services.AuthService;
using Application.Services.CategoryService;
using Application.Services.ChapterService;
using Application.Services.CourceService;
using Application.Services.FavoritService;
using Application.Services.LessonService;
using Application.Services.LessonStorageService;
using Application.Services.ReviewReactionService;
using Application.Services.ReviewService;
using Application.Services.SubscriptionService;
using Application.Services.UserService;
using Core.Models.Options;
using infrastructure.BackgroundService;
using infrastructure.Storage.Implementation;
using infrastructure.Utils.BloomFilter.implementation;
using Microsoft.Extensions.Options;
using testApi.WebUtils.HeadersService.implementation;
using testApi.WebUtils.HeadersService.interfaces;

namespace testApi.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Бизнес Сервисы
        services.AddScoped<IUsersService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourcesService>();
        services.AddScoped<IChapterService, ChapterService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IFavoritService, FavoritService>();
        services.AddScoped<ILessonStorageService, LessonsStorageService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IReviewReactionService, ReviewReactionService>();
        services.AddScoped<ICourseImageService, CourseImageService>();
        services.AddScoped<IHeaderService, HeaderService>();

        // Хранилище
        services.AddScoped<IFileStorageService, LessonFileStorageService>();
        services.AddScoped<ILessonFileService, LessonFileStorageService>();

        // AutoMapper 
        services.AddAutoMapper(typeof(UsersMapperProfile).Assembly);

        // Инструменты и Background Services
        services.AddHostedService<BloomRebuildService>();
        services.AddSingleton<IEmailChecker, EmailChecker>();

        // Настройки из Config
        services.Configure<BloomOptions>(configuration.GetSection("Bloom"));
        services.Configure<BloomRebuildOptions>(configuration.GetSection("BloomRebuild"));
        services.Configure<BackblazeOptions>(configuration.GetSection("B2"));

        // S3 Client
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<BackblazeOptions>>().Value;
            return new AmazonS3Client(
                new BasicAWSCredentials(opt.KeyId, opt.ApplicationKey),
                new AmazonS3Config { ServiceURL = opt.Endpoint, ForcePathStyle = true });
        });

        return services;
    }
}