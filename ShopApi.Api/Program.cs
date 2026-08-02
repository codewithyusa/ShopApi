using System.Text;
using System.Threading.RateLimiting;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Scalar.AspNetCore;
using HealthChecks.NpgSql;

using ShopApi.Api.ExceptionHandlers;
using ShopApi.Application.Behaviors;
using ShopApi.Application.Interfaces;
using ShopApi.Infrastructure.Auth;
using ShopApi.Infrastructure.BackgroundJobs;
using ShopApi.Infrastructure.Persistence;
using ShopApi.Infrastructure.Persistence.Repositories;
using ShopApi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register ShopDbContext with PostgreSQL
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ShopDatabase")
    ));


// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();


// Register auth services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IRefreshTokenStore, EfRefreshTokenStore>();


// Register JWT configuration and token service
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt")
);

builder.Services.AddScoped<ITokenService, TokenService>();


// Configure JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSecret = jwtSection["Secret"]!;

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

        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret)
        )
    };
});

builder.Services.AddAuthorization();


// Register Chapa payment service — currently using the FAKE implementation
// (no real HTTP calls, so no resilience pipeline needed for this one).
builder.Services.AddScoped<IChapaPaymentService, FakeChapaPaymentService>();

// Real Chapa HTTP client, pre-wired with retry + circuit breaker + timeout.
// Not active yet — IChapaPaymentService above still points to the fake.
// When real credentials are ready, change the line above to:
//   builder.Services.AddScoped<IChapaPaymentService, ChapaPaymentService>();
// and delete this registration's duplication (keep only the resilience one below).
builder.Services.AddHttpClient<ChapaPaymentService>()
    .AddResilienceHandler("chapa-pipeline", pipeline =>
    {
        pipeline.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromMilliseconds(500)
        });

        pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(30),
            MinimumThroughput = 5,
            BreakDuration = TimeSpan.FromSeconds(15)
        });

        pipeline.AddTimeout(TimeSpan.FromSeconds(10));
    });


// Register Email service — using FAKE implementation until real SMTP credentials are available.
// Logs the OTP/email content to console instead of actually sending it.
builder.Services.AddScoped<IEmailService, FakeEmailService>();


// Register MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(ShopApi.Application.Common.Result<,>).Assembly
    ));


// Register FluentValidation validators
builder.Services.AddValidatorsFromAssembly(
    typeof(ShopApi.Application.Common.Result<,>).Assembly
);


// Pipeline behaviors
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(LoggingBehavior<,>)
);

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);


// Global exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


// Health checks — hits the actual DB, not just "is the process alive"
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("ShopDatabase")!, name: "postgres");


// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Global default — most endpoints
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anon",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Tighter limiter specifically for auth endpoints — brute-force/credential-stuffing surface
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anon",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            }));
});


// Output caching — for read-heavy, infrequently-changing endpoints (product listings)
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("Products", p => p.Expire(TimeSpan.FromMinutes(2)).Tag("products"));
});


// Background jobs
builder.Services.AddHostedService<RefreshTokenCleanupService>();


// OpenAPI
builder.Services.AddOpenApi();


var app = builder.Build();


// Exception handling
app.UseExceptionHandler();


// Seed database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<ShopDbContext>();

    await DataSeeder.SeedAsync(context);

    app.MapOpenApi();
    app.MapScalarApiReference();
}


// Configure HTTP pipeline
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseOutputCache();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

app.Run();