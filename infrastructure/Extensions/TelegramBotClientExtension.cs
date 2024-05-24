using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telegram.Bot;

namespace infrastructure.Extensions;

public static class TelegramBotClientExtension
{
    public static void AddTelegramBotClient(this IServiceCollection services, string token)
    {
        services.TryAddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));
    }
}