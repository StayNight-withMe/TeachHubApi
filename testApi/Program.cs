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
using Core.Common.Types.HashId;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Interfaces.Utils;
using Core.Model.Options;
using infrastructure.BackgroundService;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Entitiеs;
using infrastructure.DataBase.Repository.Base;
using infrastructure.Storage;
using infrastructure.Storage.Implementation;
using infrastructure.UoW.implementation;
using infrastructure.Utils.BloomFilter.implementation;
using infrastructure.Utils.BloomFilter.interfaces;
using infrastructure.Utils.HashIdConverter;
using infrastructure.Utils.HeadersService;
using infrastructure.Utils.JwtService;
using infrastructure.Utils.Mapping.AutoMapperProfiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;
using System.Text.Json.Serialization;
using testApi.Middleware.Exeption;
using testApi.Middleware.RateLimit;
using testApi.Middleware.Новая_папка;
using testApi.WebUtils.HashIdConverter;
using testApi.WebUtils.JwtClaimUtil;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
var conntionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddControllers(options =>
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
    // Указываем, что Hashid — это не сложный объект для Body
    // Теперь он по умолчанию будет искаться в Route или Query
    options.SuppressInferBindingSourcesForParameters = false;
});

builder.Services.AddMvc(options =>
{
    
    options.ModelMetadataDetailsProviders.Add(new Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.ExcludeBindingMetadataProvider(typeof(Hashid)));
});




builder.Services.AddSwaggerGen(c =>
{
    
    c.MapType<Hashid>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Example = new Microsoft.OpenApi.Any.OpenApiString("jR8vWd")
    });
});

var dataSourceBuilder = new NpgsqlDataSourceBuilder(conntionString);


dataSourceBuilder.EnableDynamicJson();

var dataSource = dataSourceBuilder.Build();

// залупа1
builder.Services.AddHostedService<BloomRebuildService>();
builder.Services.AddDbContext<CourceDbContext>(options => options.UseNpgsql(dataSource));

builder.Services.AddScoped<JwtClaimUtil>();

//репозитории

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
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
//builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<UsersMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<RoleMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AuthMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<CoursesMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ChaptermMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<LessonMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ReviewMapperProfile>());
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ReviewReactionMapperProfile>());

builder.Services.AddScoped<ILessonFileService, LessonFileStorageService>();
builder.Services.AddScoped<ICourseImageService, CourseImageService>();



builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;  
    o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    o.ReportApiVersions = true;  
});


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


var app = builder.Build();
app.UseMiddleware<RateLimitMiddleware>(60, 20);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();        // даёт JSON-документ по /swagger/v1/swagger.json
    app.UseSwaggerUI();
}

app.UseMiddleware<ExeptionMiddleware>();

app.UseHttpsRedirection();

//app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.UseMiddleware<IpValidateMidlleware>();



app.MapControllers();

app.Run();
