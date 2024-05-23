using fetch_latest_articles_worker.Services;

namespace fetch_latest_articles_worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FetchLatestArticlesService _fetchLatestArticlesService;

    public Worker(ILogger<Worker> logger, FetchLatestArticlesService fetchLatestArticlesService)
    {
        _logger = logger;
        _fetchLatestArticlesService = fetchLatestArticlesService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var articles = await _fetchLatestArticlesService.Fetch();

            await Task.Delay(1000, stoppingToken);
        }
    }
}