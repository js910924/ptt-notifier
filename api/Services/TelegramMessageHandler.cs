using domain.Models;
using infrastructure;
using Telegram.Bot;

namespace api.Services;

public class TelegramMessageHandler : ITelegramMessageHandler
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscribedBoardRepository _subscribedBoardRepository;
    private readonly IPttClient _pttClient;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IArticleRepository _articleRepository;
    private readonly ILogger<TelegramMessageHandler> _logger;

    public TelegramMessageHandler(ISubscriptionRepository subscriptionRepository, ISubscribedBoardRepository subscribedBoardRepository, IPttClient pttClient, ITelegramBotClient telegramBotClient, IArticleRepository articleRepository, ILogger<TelegramMessageHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscribedBoardRepository = subscribedBoardRepository;
        _pttClient = pttClient;
        _telegramBotClient = telegramBotClient;
        _articleRepository = articleRepository;
        _logger = logger;
    }

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

        await _telegramBotClient.SendTextMessageAsync(chatId, replyText);
    }

    private async Task<string> ListSubscription(long chatId)
    {
        try
        {
            var subscriptions = await _subscriptionRepository.Get(chatId);

            return string.Join('\n', subscriptions.Select(subscription => $"{subscription.Board} {subscription.Keyword}"));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "List subscriptions failed");
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
    
            await _subscriptionRepository.Delete(chatId, board, keyword);
            var subscriptions = await _subscriptionRepository.GetAll();
            if (subscriptions.TrueForAll(subscription => subscription.Board != board))
            {
                await _subscribedBoardRepository.Delete(board);
                await _articleRepository.Delete(board);
            }

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
    
            await _subscriptionRepository.Add(chatId, board, keyword);
            var subscribedBoards = await _subscribedBoardRepository.GetAll();
            if (!subscribedBoards.Exists(b => b.Board == board))
            {
                var latestArticle = await _pttClient.GetLatestArticle(board);
                var subscribedBoard = new SubscribedBoard
                {
                    Board = board,
                    LastLatestArticleTitle = latestArticle.Title
                };
                await _subscribedBoardRepository.Add(subscribedBoard);
            }

            return "Subscribe successfully.";
        }

        return "Invalid command. Use /subscribe [board] [keyword]";
    }
}