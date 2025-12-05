using Applcation.Service.AuthService;
using Applcation.Service.chapterService;
using Applcation.Service.CourceService;
using Applcation.Service.UserService;
using Applcation.Service.LessonService;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Interfaces.Utils;
using infrastructure.Context;
using infrastructure.Entitiеs;
using infrastructure.Repository.Base;
using infrastructure.UoW.implementation;
using infrastructure.Utils.HeadersService;
using infrastructure.Utils.JwtService;
using infrastructure.Utils.Mapping.AutoMapperProfiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using testApi.Middleware.Exeption;
using testApi.Middleware.RateLimit;
using testApi.Middleware.Новая_папка;
using Applcation.Service.SubscriptionService;
using Applcation.Service.FavoritService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
var conntionString = builder.Configuration.GetConnectionString("DefaultConnection");

// залупа1
builder.Services.AddDbContext<CourceDbContext>(options => options.UseNpgsql(conntionString));

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

app.UseHttpsRedirection();

//app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.UseMiddleware<IpValidateMidlleware>();
app.UseMiddleware<ExeptionMiddleware>();


app.MapControllers();

app.Run();
