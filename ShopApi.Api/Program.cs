using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Behaviors;
using ShopApi.Application.Interfaces;
using ShopApi.Infrastructure.Persistence;
using ShopApi.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register ShopDbContext with PostgreSQL
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ShopDatabase")
    ));

// Register repositories (scoped — matches ShopDbContext's lifetime)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();

// Register MediatR (scans Application assembly for commands/queries/handlers)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ShopApi.Application.Common.Result<,>).Assembly));

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssembly(typeof(ShopApi.Application.Common.Result<,>).Assembly);

// Pipeline behaviors — LoggingBehavior first so it wraps ValidationBehavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed database in development environment
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<ShopDbContext>();

    await DataSeeder.SeedAsync(context);

    app.MapOpenApi();
}

// Configure HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();