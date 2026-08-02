using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShopApi.Infrastructure.Persistence;

namespace ShopApi.Infrastructure.BackgroundJobs;

public class RefreshTokenCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<RefreshTokenCleanupService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ShopDbContext>();

                var cutoff = DateTime.UtcNow.AddDays(-30);
                var deleted = await context.RefreshTokens
                    .Where(r => r.ExpiresAt < DateTime.UtcNow &&
                                (r.RevokedAt == null || r.RevokedAt < cutoff))
                    .ExecuteDeleteAsync(stoppingToken);

                if (deleted > 0)
                    logger.LogInformation("Refresh token cleanup removed {Count} rows", deleted);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Refresh token cleanup failed");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}