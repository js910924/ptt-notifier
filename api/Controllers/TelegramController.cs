using api.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TelegramController(ITelegramMessageHandler telegramMessageHandler, ILogger<TelegramController> logger) : Controller
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

            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Handle webhook failed");
            return Ok("process request failed, please try again later...");
        }
    }
}