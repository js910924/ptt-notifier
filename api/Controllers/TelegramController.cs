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
            await telegramBotClient.SendTextMessageAsync(update.Message!.Chat.Id, "Unexpected Error");
        }

        return Ok();
    }
}