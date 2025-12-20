using Amazon.Runtime;
using Amazon.S3;
using Applcation.Service.AuthService;
using Applcation.Service.CategoryService;
using Applcation.Service.chapterService;
using Applcation.Service.CourceService;
using Applcation.Service.FavoritService;
using Applcation.Service.LessonService;
using Applcation.Service.LessonStorageService;
using Applcation.Service.ReviewReactionService;
using Applcation.Service.ReviewService;
using Applcation.Service.SubscriptionService;
using Applcation.Service.UserService;
using Core.Common;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Interfaces.Utils;
using Core.Model.Options;
using infrastructure.BackgroundService;
using infrastructure.Context;
using infrastructure.Entitiеs;
using infrastructure.Repository.Base;
using infrastructure.Storage;
using infrastructure.Storage.Implementation;
using infrastructure.UoW.implementation;
using infrastructure.Utils.BloomFilter.implementation;
using infrastructure.Utils.BloomFilter.interfaces;
using infrastructure.Utils.HeadersService;
using infrastructure.Utils.JwtService;
using infrastructure.Utils.Mapping.AutoMapperProfiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Text;
using System.Text.Json.Serialization;
using testApi.Middleware.Exeption;
using testApi.Middleware.RateLimit;
using testApi.Middleware.Новая_папка;
using static infrastructure.Utils.BloomFilter.interfaces.IEmailChecker;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
var conntionString = builder.Configuration.GetConnectionString("DefaultConnection");



var dataSourceBuilder = new NpgsqlDataSourceBuilder(conntionString);
dataSourceBuilder.MapEnum<reaction_type>(); // Название типа в C#
var dataSource = dataSourceBuilder.Build();

// залупа1
builder.Services.AddHostedService<BloomRebuildService>();
builder.Services.AddDbContext<CourceDbContext>(options => options.UseNpgsql(dataSource));



//репозитории
builder.Services.AddScoped<IBaseRepository<UserEntities>, BaseRepository<UserEntities>>();
builder.Services.AddScoped<IBaseRepository<UserRoleEntities>, BaseRepository<UserRoleEntities>>();
builder.Services.AddScoped<IBaseRepository<RoleEntities>, BaseRepository<RoleEntities>>();
builder.Services.AddScoped<IBaseRepository<CourseEntities>, BaseRepository<CourseEntities>>();
builder.Services.AddScoped<IBaseRepository<ChapterEntity>, BaseRepository<ChapterEntity>>();
builder.Services.AddScoped<IBaseRepository<LessonEntities>, BaseRepository<LessonEntities>>();
builder.Services.AddScoped<IBaseRepository<LessonEntities>, BaseRepository<LessonEntities>>();
builder.Services.AddScoped<IBaseRepository<SubscriptionEntites>, BaseRepository<SubscriptionEntites>>();
builder.Services.AddScoped<IBaseRepository<FavoritEntities>, BaseRepository<FavoritEntities>>();
builder.Services.AddScoped<IBaseRepository<LessonfilesEntities>, BaseRepository<LessonfilesEntities>>();
builder.Services.AddScoped<IBaseRepository<CategoriesEntities>, BaseRepository<CategoriesEntities>>();
builder.Services.AddScoped<IBaseRepository<Course_CategoriesEntities>, BaseRepository<Course_CategoriesEntities>>();
builder.Services.AddScoped<IBaseRepository<ReviewEntities>, BaseRepository<ReviewEntities>>();
builder.Services.AddScoped<IBaseRepository<ReviewreactionEntities>, BaseRepository<ReviewreactionEntities>>();
//сервисы
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUsersService ,UserService>();
builder.Services.AddSingleton<IJwtService , JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourseService, CourcesService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IFavoritService, FavoritService>();
builder.Services.AddScoped<ILessonStorageService, LessonsStorageService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewReactionService, ReviewReactionService>();
//залупа2
builder.Services.AddEndpointsApiExplorer(); //свагеру что бы найти
builder.Services.AddSwaggerGen(); // свагеру для создания документа
builder.Logging.AddConsole();
//внешние иснтрументы
builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<UsersMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<RoleMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AuthMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<CoursesMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ChaptermMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<LessonMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ReviewMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ReviewReactionMapperProfile>());







builder.Services.Configure<BloomOptions>(
    builder.Configuration.GetSection("Bloom"));

builder.Services.Configure<BloomRebuildOptions>(
    builder.Configuration.GetSection("BloomRebuild"));

builder.Services.AddSingleton<IEmailChecker, EmailChecker>();

builder.Services.AddScoped<IFileStorageService, LessonFileStorageService>();


builder.Services.Configure<BackblazeOptions>(
    builder.Configuration.GetSection("B2"));



builder.Services.AddSingleton<IAmazonS3>(
    sp =>
    {
        var opt = sp.GetRequiredService<IOptions<BackblazeOptions>>().Value;
        return new AmazonS3Client(
            new BasicAWSCredentials(opt.KeyId, opt.ApplicationKey),
            new AmazonS3Config { ServiceURL = opt.Endpoint, ForcePathStyle = true });
    });
builder.Services.AddScoped<IHeaderService, HeaderService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

     
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
    };

   
        
     });


builder.Services.AddOutputCache(opt =>
{
    opt.AddPolicy("1min", poli => poli.Expire(TimeSpan.FromSeconds(60))
    .SetVaryByQuery("*")
    .SetVaryByHeader("Authorization")
    .Tag("1min")
    );

    opt.AddPolicy("10min", poli => poli.Expire(TimeSpan.FromMinutes(10))
    .SetVaryByQuery("*")
    .SetVaryByHeader("Authorization")
    .Tag("10min")
    );

    opt.AddPolicy("30min", poli => poli.Expire(TimeSpan.FromMinutes(30))
    .SetVaryByQuery("*")
    .Tag("30min")
    );


    opt.AddPolicy("60min", poli => poli.Expire(TimeSpan.FromMinutes(60))
    .SetVaryByQuery("*")
    .Tag("60min")
    );

    opt.AddPolicy("CheckEmail1Hour", c => c.Expire(TimeSpan.FromMinutes(60))
    .SetVaryByQuery("*")
    .Tag("email-check")
    

);

});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();
app.UseMiddleware<RateLimitMiddleware>(60, 20);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();        // даёт JSON-документ по /swagger/v1/swagger.json
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.UseMiddleware<IpValidateMidlleware>();
app.UseMiddleware<ExeptionMiddleware>();


app.MapControllers();

app.Run();
