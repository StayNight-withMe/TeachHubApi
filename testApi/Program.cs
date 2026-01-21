using testApi.Middleware.Exeption;
using testApi.Middleware.IpValidate;
using testApi.Middleware.RateLimit;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();
builder.Services.AddDatabaseInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddWebUiServices();

builder.Logging.AddConsole();

var app = builder.Build();


app.UseMiddleware<RateLimitMiddleware>(60, 20);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExeptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<IpValidateMidlleware>();

app.MapControllers();

app.Run();