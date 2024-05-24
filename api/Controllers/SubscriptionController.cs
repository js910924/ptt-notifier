using api.Requests;
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
public class SubscriptionController : Controller
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscribedBoardRepository _subscribedBoardRepository;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IPttClient _pttClient;

    public SubscriptionController(ISubscriptionRepository subscriptionRepository, ISubscribedBoardRepository subscribedBoardRepository, ITelegramBotClient telegramBotClient, IPttClient pttClient)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscribedBoardRepository = subscribedBoardRepository;
        _telegramBotClient = telegramBotClient;
        _pttClient = pttClient;
    }

    [HttpPost]
    public async Task<List<Subscription>> Subscribe(SubscribeRequest request)
    {
        await _subscriptionRepository.Add(request.UserId, request.Board, request.Keyword);
        var subscribedBoards = await _subscribedBoardRepository.GetAll();
        if (!subscribedBoards.Exists(board => board.Board == request.Board))
        {
            var latestArticle = await _pttClient.GetLatestArticle(request.Board);
            var subscribedBoard = new SubscribedBoard
            {
                Board = request.Board,
                LastLatestArticleTitle = latestArticle.Title
            };
            await _subscribedBoardRepository.Add(subscribedBoard);
        }

        return await _subscriptionRepository.GetAll();
    }

    [HttpDelete]
    public async Task<OkResult> Unsubscribe(SubscribeRequest request)
    {
        await _subscriptionRepository.Delete(request.UserId, request.Board, request.Keyword);
        var subscriptions = await _subscriptionRepository.GetAll();
        if (subscriptions.All(subscription => subscription.Board != request.Board))
        {
            await _subscribedBoardRepository.Delete(request.Board);
        }

        return Ok();
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
            var messageText = update.Message.Text.Split(' ');
            if (messageText.Length == 3 && messageText[0].ToLower() == "/subscribe")
            {
                var userId = update.Message.Chat.Id;
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
        
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id,
                    "Subscription added successfully.");
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id,
                    "Invalid command. Use /subscribe [board] [keyword]");
            }
        }

        return Ok();
    }
}