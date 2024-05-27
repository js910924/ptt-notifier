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
        if (message.StartsWith("/subscribe"))
        {
            var messageText = message.Split(' ');
            if (messageText.Length == 3 && messageText[0].ToLower() == "/subscribe")
            {
                // TODO: validate is valid board 
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
    
                await _telegramBotClient.SendTextMessageAsync(chatId, "Subscribe successfully.");
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(chatId, "Invalid command. Use /subscribe [board] [keyword]");
            }
        }
        else if (message.StartsWith("/unsubscribe"))
        {
            var messageText = message.Split(' ');
            if (messageText.Length == 3 && messageText[0].ToLower() == "/unsubscribe")
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
    
                await _telegramBotClient.SendTextMessageAsync(chatId,
                    "Unsubscribe successfully.");
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(chatId,
                    "Invalid command. Use /unsubscribe [board] [keyword]");
            }
        }
        else if (message.Equals("/list", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var subscriptions = await _subscriptionRepository.Get(chatId);
                var text = string.Join('\n', subscriptions.Select(subscription => $"{subscription.Board} {subscription.Keyword}"));

                await _telegramBotClient.SendTextMessageAsync(chatId, text);
            }
            catch (Exception e)
            {
                await _telegramBotClient.SendTextMessageAsync(chatId, "Unexpected error");
                _logger.LogError(e, "List subscriptions failed");
            }
        }
    }
}