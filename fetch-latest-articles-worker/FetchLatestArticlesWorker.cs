using fetch_latest_articles_worker.Services;
using infrastructure;

namespace fetch_latest_articles_worker;

public class FetchLatestArticlesWorker : BackgroundService
{
    private readonly ILogger<FetchLatestArticlesWorker> _logger;
    private readonly FetchLatestArticlesService _fetchLatestArticlesService;
    private readonly ISubscribedBoardRepository _subscribedBoardRepository;
    private readonly IArticleRepository _articleRepository;
    private const int MillisecondsDelay = 5000;

    public FetchLatestArticlesWorker(ILogger<FetchLatestArticlesWorker> logger, FetchLatestArticlesService fetchLatestArticlesService, ISubscribedBoardRepository subscribedBoardRepository, IArticleRepository articleRepository)
    {
        _logger = logger;
        _fetchLatestArticlesService = fetchLatestArticlesService;
        _subscribedBoardRepository = subscribedBoardRepository;
        _articleRepository = articleRepository;
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
                await _articleRepository.Add(latestArticles);
            }

            await Task.Delay(MillisecondsDelay, stoppingToken);
        }
    }
}