namespace api.Services;

public interface ITelegramMessageHandler
{
    Task Handle(long chatId, string message);
}