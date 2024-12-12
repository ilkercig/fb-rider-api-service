using FbRider.Api;
using FbRider.Api.Middlewares;
using FbRider.Api.Options;
using FbRider.Api.Repositories;
using FbRider.Api.Services;
using FbRider.Api.YahooApi;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); // Replace default logger with Serilog



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true; // Ensures the cookie is not accessible via JavaScript
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requires HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax; // Lax for cross-site requests with cookies
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Cookie expires in 30 days
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddDistributedMemoryCache(); // Adds an in-memory implementation of IDistributedCache

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout
    options.Cookie.HttpOnly = true; // Make cookie accessible only through HTTP
    options.Cookie.IsEssential = true; // Mark the cookie as essential
});

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
else if (builder.Environment.IsProduction())
{
    // Add production services
}

// Bind the secrets to SecretsOptions and validate them
builder.Services.Configure<SecretsOptions>(
    builder.Configuration.GetSection("Secrets")
);
builder.Services.AddOptions<SecretsOptions>()
    .Bind(builder.Configuration.GetSection("Secrets"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IYahooSignInApiClient, YahooSignInApiClient>();
builder.Services.AddSingleton<IYahooFantasySportsApiClient, YahooFantasySportsApiClient>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    //var environment = builder.Environment.EnvironmentName; 
    options.UseNpgsql(connectionString);
    
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(); // Log HTTP requests
app.UseHttpsRedirection();
app.UseSession(); // Add session middleware
app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TokenRefreshMiddleware>();

app.Run();
