using Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

namespace Vasconlabs.Aeolus.Interface.Server.Services;

public class AeolusSnapshotService(ICacheOperations cacheOperations): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await cacheOperations.SaveSnapshot().ConfigureAwait(false);
            
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}