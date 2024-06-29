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

            return string.Join('\n', subscriptions.Select(subscription => $"{subscription.Board} {subscription.Keyword} {subscription.Author}"));
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
        if (messageText.Length == 4 && messageText[0].Equals("/unsubscribe", StringComparison.CurrentCultureIgnoreCase))
        {
            var target = messageText[1];
            var board = messageText[2];
            var keyword = messageText[3];
    
            if (target.Equals("article", StringComparison.OrdinalIgnoreCase))
            {
                await subscriptionService.Unsubscribe(chatId, board, keyword);
            }
            else if (target.Equals("author", StringComparison.OrdinalIgnoreCase))
            {
                await subscriptionService.UnsubscribeAuthor(chatId, board, keyword);
            }
            else
            {
                return "Invalid command. Use /unsubscribe [target] [board] [keyword]";
            }


            return "Unsubscribe successfully.";
        }

        return "Invalid command. Use /unsubscribe [target] [board] [keyword]";
    }

    private async Task<string> Subscribe(long chatId, string message)
    {
        var messageText = message.Split(' ');
        if (messageText.Length == 4 && messageText[0].Equals("/subscribe", StringComparison.CurrentCultureIgnoreCase))
        {
            var target = messageText[1];
            var board = messageText[2];
            var keyword = messageText[3];

            if (target.Equals("article", StringComparison.OrdinalIgnoreCase))
            {
                await subscriptionService.Subscribe(chatId, board, keyword);
            }
            else if (target.Equals("author", StringComparison.OrdinalIgnoreCase))
            {
                await subscriptionService.SubscribeAuthor(chatId, board, keyword);
            }
            else
            {
                return "Invalid command. Use /subscribe [target] [board] [keyword]";
            }

            return "Subscribe successfully.";
        }

        return "Invalid command. Use /subscribe [target] [board] [keyword]";
    }
}