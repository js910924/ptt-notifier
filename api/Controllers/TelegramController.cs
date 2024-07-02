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
    public const string HelpMessage = "新增訂閱\n" +
                                      "/subscribe [target] [board] [keyword]\n" +
                                      "訂閱 Stock 板任何有 \"2330\" 在標題的文章\n" +
                                      "/subscribe article stock 2330\n" +
                                      "訂閱 Stock 板 \"anyAuthor\" 這位作者的任何文章\n" +
                                      "/subscribe author stock anyAuthor\n" +
                                      "取消訂閱\n" +
                                      "/unsubscribe [target] [board] [keyword]\n" +
                                      "列出訂閱\n" +
                                      "/list\n" +
                                      "取得幫助\n" +
                                      "/help";

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
            await telegramBotClient.SendTextMessageAsync(update.Message!.Chat.Id, $"--- Invalid Command ---\n\n{HelpMessage}");
        }

        return Ok();
    }
}