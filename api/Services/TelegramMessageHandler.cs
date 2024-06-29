using infrastructure;
using Telegram.Bot;

namespace api.Services;

public class TelegramMessageHandler(
    ISubscriptionRepository subscriptionRepository,
    ITelegramBotClient telegramBotClient,
    ILogger<TelegramMessageHandler> logger,
    ISubscriptionService subscriptionService)
    : ITelegramMessageHandler
{
    public async Task Handle(long chatId, string message)
    {
        var replyText = string.Empty;
        if (message.StartsWith("/subscribe"))
        {
            replyText = await Subscribe(chatId, message);
        }
        else if (message.StartsWith("/unsubscribe"))
        {
            replyText = await Unsubscribe(chatId, message);
        }
        else if (message.Equals("/list", StringComparison.OrdinalIgnoreCase))
        {
            replyText = await ListSubscription(chatId);
        }

        await telegramBotClient.SendTextMessageAsync(chatId, replyText);
    }

    private async Task<string> ListSubscription(long chatId)
    {
        try
        {
            var subscriptions = await subscriptionRepository.Get(chatId);

            return string.Join('\n', subscriptions.Select(subscription => $"{subscription.Board} {subscription.Keyword}"));
        }
        catch (Exception e)
        {
            logger.LogError(e, "List subscriptions failed");
            return "Unexpected error";
        }
    }

    private async Task<string> Unsubscribe(long chatId, string message)
    {
        var messageText = message.Split(' ');
        if (messageText.Length == 3 && messageText[0].Equals("/unsubscribe", StringComparison.CurrentCultureIgnoreCase))
        {
            var board = messageText[1];
            var keyword = messageText[2];
    
            await subscriptionService.Unsubscribe(chatId, board, keyword);

            return "Unsubscribe successfully.";
        }

        return "Invalid command. Use /unsubscribe [board] [keyword]";
    }

    private async Task<string> Subscribe(long chatId, string message)
    {
        var messageText = message.Split(' ');
        if (messageText.Length == 3 && messageText[0].Equals("/subscribe", StringComparison.CurrentCultureIgnoreCase))
        {
            var board = messageText[1];
            var keyword = messageText[2];

            await subscriptionService.Subscribe(chatId, board, keyword);

            return "Subscribe successfully.";
        }

        return "Invalid command. Use /subscribe [board] [keyword]";
    }
}