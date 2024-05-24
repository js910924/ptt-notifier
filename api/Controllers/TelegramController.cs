using domain.Models;
using infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TelegramController : Controller
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscribedBoardRepository _subscribedBoardRepository;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IPttClient _pttClient;

    public TelegramController(ISubscriptionRepository subscriptionRepository, ISubscribedBoardRepository subscribedBoardRepository, ITelegramBotClient telegramBotClient, IPttClient pttClient)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscribedBoardRepository = subscribedBoardRepository;
        _telegramBotClient = telegramBotClient;
        _pttClient = pttClient;
    }

    [HttpPost]
    public async Task<IActionResult> TelegramWebhook()
    {
        // TODO: not sure why use Update as parameter will get 400 Bad Request
        using StreamReader reader = new(Request.Body);
        var bodyAsString = await reader.ReadToEndAsync();
        var update = JsonConvert.DeserializeObject<Update>(bodyAsString);

        if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
        {
            if (update.Message.Text.StartsWith("/subscribe"))
            {
                await Subscribe(update.Message);
            }
        }

        return Ok();
    }

    private async Task Subscribe(Message message)
    {
        var messageText = message.Text.Split(' ');
        if (messageText.Length == 3 && messageText[0].ToLower() == "/subscribe")
        {
            var userId = message.Chat.Id;
            // TODO: validate is valid board 
            var board = messageText[1];
            var keyword = messageText[2];
    
            await _subscriptionRepository.Add(userId, board, keyword);
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
    
            await _telegramBotClient.SendTextMessageAsync(message.Chat.Id,
                "Subscription added successfully.");
        }
        else
        {
            await _telegramBotClient.SendTextMessageAsync(message.Chat.Id,
                "Invalid command. Use /subscribe [board] [keyword]");
        }
    }
}