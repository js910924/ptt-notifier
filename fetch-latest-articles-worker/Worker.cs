using fetch_latest_articles_worker.Services;
using infrastructure;

namespace fetch_latest_articles_worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FetchLatestArticlesService _fetchLatestArticlesService;
    private readonly ISubscribedBoardRepository _subscribedBoardRepository;

    public Worker(ILogger<Worker> logger, FetchLatestArticlesService fetchLatestArticlesService, ISubscribedBoardRepository subscribedBoardRepository)
    {
        _logger = logger;
        _fetchLatestArticlesService = fetchLatestArticlesService;
        _subscribedBoardRepository = subscribedBoardRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var subscribedBoards = await _subscribedBoardRepository.GetAll();
            foreach (var subscribedBoard in subscribedBoards)
            {
                var latestArticles = await _fetchLatestArticlesService.Fetch(subscribedBoard);
                // TODO: store to database
            }

            // var subscriptions = await _subscriptionRepository.Get(subscribedBoard);
            // foreach (var subscription in subscriptions)
            // {
            //     var targetArticles = latestArticles.Where(article => article.Title.Contains(subscription.Keyword, StringComparison.OrdinalIgnoreCase));
            //     await _telegramBotClient.SendTextMessageAsync(subscription.UserId, string.Join('\n', targetArticles.Select(article => $"{article.Title}\n{article.Link}")), cancellationToken: stoppingToken);
            // }
            await Task.Delay(1000, stoppingToken);
        }
    }
}