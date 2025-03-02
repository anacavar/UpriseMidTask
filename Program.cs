using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Text;
using UpriseMidTask.Data;


var builder = WebApplication.CreateBuilder(args);

Env.Load();

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{

    logger.Debug("Initializing application");

    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
    var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true; 
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

    builder.Services.AddAuthentication()
        .AddJwtBearer("some-scheme", jwtOptions =>
        {
            jwtOptions.MetadataAddress = builder.Configuration["Api:MetadataAddress"]; 

            jwtOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = builder.Configuration["Jwt:Audience"], 
                ValidIssuer = builder.Configuration["Jwt:Issuer"], 
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };

            jwtOptions.MapInboundClaims = false;
        });

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    builder.Host.UseNLog();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch
{
    logger.Error("An error occurred while initializing the application.");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
