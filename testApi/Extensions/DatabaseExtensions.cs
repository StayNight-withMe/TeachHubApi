using Application.Abstractions.Repository.Base;
using Application.Abstractions.Repository.Custom;
using Application.Abstractions.UoW;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Repository.Base;
using infrastructure.DataBase.Repository.Custom;
using infrastructure.UoW.implementation;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Microsoft.Extensions.DependencyInjection;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseInfrastructure(this IServiceCollection services, string connectionString)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<CourceDbContext>(options => options.UseNpgsql(dataSource));

        // Репозитории
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IChapterRepository, ChapterRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<ILessonFileRepository, LessonFileRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}