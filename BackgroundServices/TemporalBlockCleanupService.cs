using Modeer360.Repositories;

namespace Modeer360.BackgroundServices;

public class TemporalBlockCleanupService : BackgroundService
{
    private readonly InMemoryStore _store;
    private readonly ILogger<TemporalBlockCleanupService> _logger;

    public TemporalBlockCleanupService(InMemoryStore store, ILogger<TemporalBlockCleanupService> logger)
    {
        _store = store;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _store.RemoveExpiredTemporalBlocks();
            _logger.LogInformation("Temporal block cleanup ran at {Time}", DateTime.UtcNow);

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}