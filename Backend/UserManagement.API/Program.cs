using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using UserManagement.API.Middleware;
using UserManagement.API.Services;
using UserManagement.Application.Common;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Application.Services;
using UserManagement.Infrastructure.Persistance;
using UserManagement.Infrastructure.Persistance.Respositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings.json
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // Credentialed (cookie-based) requests can't use a wildcard/any-origin policy -
        // the frontend's origin(s) must be listed explicitly.
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRespository>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Removes default 5 min grace period for token expiration
    };

    // The token is stored in an HttpOnly cookie (not sent as an Authorization header),
    // so pull it from there instead.
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue(AuthCookieNames.AccessToken, out var token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService,RoleServices>();
builder.Services.AddScoped<IJWTGeneratorService, JWTGeneratorService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseMiddleware<XsrfValidationMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


//dotnet ef migrations add InitialMigarations --project .\UserManagement.Infrastructure\ --startup-project .\UserManagement.API\ --output-dir Persistance\Migrations\ --context ApplicationDbContext
//dotnet ef database update --project .\UserManagement.Infrastructure\ --startup-project .\UserManagement.API\
