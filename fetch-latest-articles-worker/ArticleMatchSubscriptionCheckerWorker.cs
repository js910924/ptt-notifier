using infrastructure;
using Telegram.Bot;

namespace fetch_latest_articles_worker;

public class ArticleMatchSubscriptionCheckerWorker(
    ILogger<ArticleMatchSubscriptionCheckerWorker> logger,
    IArticleRepository articleRepository,
    ISubscriptionRepository subscriptionRepository,
    ITelegramBotClient telegramBotClient)
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

            var articlesByBoard = (await articleRepository.GetAll())
                .GroupBy(article => article.Board)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());
            foreach (var keyValuePair in articlesByBoard)
            {
                var subscriptions = await subscriptionRepository.Get(keyValuePair.Key);
                foreach (var article in keyValuePair.Value)
                {
                    var matchedSubscriptions = subscriptions
                        .Where(subscription =>
                            subscription.Keyword is not null && article.Title.Contains(subscription.Keyword, StringComparison.OrdinalIgnoreCase)
                            || subscription.Keyword is null && article.Author == subscription.Author);
                    var formatedArticle = string.Join('\n', $"{article.Title}\n{article.Link}");
                    var tasks = matchedSubscriptions.Select(subscription =>
                        telegramBotClient.SendTextMessageAsync(subscription.UserId, formatedArticle, cancellationToken: stoppingToken));

                    await Task.WhenAll(tasks);
                    await articleRepository.Delete(article.Id);
                }
            }

            await Task.Delay(MillisecondsDelay, stoppingToken);
        }
    }
}