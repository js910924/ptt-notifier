using api.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TelegramController : Controller
{
    private readonly ITelegramMessageHandler _telegramMessageHandler;

    public TelegramController(ITelegramMessageHandler telegramMessageHandler)
    {
        _telegramMessageHandler = telegramMessageHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Webhook()
    {
        // TODO: not sure why use Update as parameter will get 400 Bad Request
        using StreamReader reader = new(Request.Body);
        var bodyAsString = await reader.ReadToEndAsync();
        var update = JsonConvert.DeserializeObject<Update>(bodyAsString);

        if (update is { Type: UpdateType.Message, Message: { Type: MessageType.Text, Text: not null } })
        {
            await _telegramMessageHandler.Handle(update.Message.Chat.Id, update.Message.Text);
        }

        return Ok();
    }
}