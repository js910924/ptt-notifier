using infrastructure.Configs;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace api;

public class SetTelegramWebhookHostedService(IOptions<TelegramConfig> options, ITelegramBotClient telegramBotClient)
    : IHostedService
{
    private readonly TelegramConfig _telegramConfig = options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await telegramBotClient.SetWebhookAsync(_telegramConfig.WebhookUrl, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}