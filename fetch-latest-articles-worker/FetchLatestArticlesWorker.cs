using fetch_latest_articles_worker.Services;
using infrastructure;

namespace fetch_latest_articles_worker;

public class FetchLatestArticlesWorker(
    ILogger<FetchLatestArticlesWorker> logger,
    FetchLatestArticlesService fetchLatestArticlesService,
    ISubscribedBoardRepository subscribedBoardRepository,
    IArticleRepository articleRepository)
    : BackgroundService
{
    private const int MillisecondsDelay = 5000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var subscribedBoards = await subscribedBoardRepository.GetAll();
            foreach (var subscribedBoard in subscribedBoards)
            {
                var latestArticles = await fetchLatestArticlesService.Fetch(subscribedBoard);
                await articleRepository.Add(latestArticles);
            }

            await Task.Delay(MillisecondsDelay, stoppingToken);
        }
    }
}