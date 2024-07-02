using api.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TelegramController(ITelegramMessageHandler telegramMessageHandler, ILogger<TelegramController> logger, ITelegramBotClient telegramBotClient) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Webhook(Update update)
    {
        try
        {
            if (update is { Type: UpdateType.Message, Message: { Type: MessageType.Text, Text: not null } })
            {
                await telegramMessageHandler.Handle(update.Message.Chat.Id, update.Message.Text);
            }

        }
        catch (CommandException e)
        {
            logger.LogError(e, "{Message}", update.Message!.Text);
            await telegramBotClient.SendTextMessageAsync(update.Message!.Chat.Id, e.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Handle webhook failed");
            await telegramBotClient.SendTextMessageAsync(update.Message!.Chat.Id, "--- Invalid Command ---\n\n新增訂閱\n/subscribe [target] [board] [keyword]\n取消訂閱\n/unsubscribe [target] [board] [keyword]\n列出訂閱\n/list\n已支援 target: article, author");
        }

        return Ok();
    }
}