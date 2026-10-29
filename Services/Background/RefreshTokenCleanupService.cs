using Library_API.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Services.Background
{
    public sealed class RefreshTokenCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<RefreshTokenCleanupService> logger) 
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var db = scope.ServiceProvider
                        .GetRequiredService<AppDbContext>();

                    var now = DateTime.UtcNow;
                    var retentionCutoff = now.AddDays(-15);

                    var deleted = await db.RefreshTokens
                   .Where(t =>
                       t.ExpiresAtUtc < now &&
                       (t.RevokedAtUtc == null ||
                        t.RevokedAtUtc < retentionCutoff))
                   .ExecuteDeleteAsync(stoppingToken);

                    logger.LogInformation(
                        "Refresh token cleanup deleted {Count} records",
                        deleted);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex, "Refresh token cleanup failed");
                }
            }
        }
    }
}
