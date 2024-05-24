using fetch_latest_articles_worker.Services;
using infrastructure;
using Telegram.Bot;

namespace fetch_latest_articles_worker;

public class ArticleMatchSubscriptionCheckerWorker : BackgroundService
{
    private readonly ILogger<ArticleMatchSubscriptionCheckerWorker> _logger;
    private readonly IArticleRepository _articleRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ITelegramBotClient _telegramBotClient;

    public ArticleMatchSubscriptionCheckerWorker(ILogger<ArticleMatchSubscriptionCheckerWorker> logger, FetchLatestArticlesService fetchLatestArticlesService, ISubscribedBoardRepository subscribedBoardRepository, IArticleRepository articleRepository, ISubscriptionRepository subscriptionRepository, ITelegramBotClient telegramBotClient)
    {
        _logger = logger;
        _articleRepository = articleRepository;
        _subscriptionRepository = subscriptionRepository;
        _telegramBotClient = telegramBotClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var articlesByBoard = (await _articleRepository.GetAll()).GroupBy(article => article.Board)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());
            foreach (var keyValuePair in articlesByBoard)
            {
                var subscriptions = await _subscriptionRepository.Get(keyValuePair.Key);
                foreach (var article in keyValuePair.Value)
                {
                    var matchedSubscriptions = subscriptions.Where(subscription => article.Title.Contains(subscription.Keyword, StringComparison.OrdinalIgnoreCase));
                    var formatedArticle = string.Join('\n', $"{article.Title}\n{article.Link}");
                    var tasks = matchedSubscriptions.Select(subscription =>
                        _telegramBotClient.SendTextMessageAsync(subscription.UserId, formatedArticle,
                            cancellationToken: stoppingToken));

                    await Task.WhenAll(tasks);
                    await _articleRepository.Delete(article.Id);
                }
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}