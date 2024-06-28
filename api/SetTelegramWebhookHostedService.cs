using infrastructure.Configs;
using Microsoft.Extensions.Options;

namespace api;

public class SetTelegramWebhookHostedService : IHostedService
{
    private readonly HttpClient _httpClient;
    private readonly TelegramConfig _telegramConfig;

    public SetTelegramWebhookHostedService(HttpClient httpClient, IOptions<TelegramConfig> options)
    {
        _httpClient = httpClient;
        _telegramConfig = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var setWebhookUrl = $"{_telegramConfig.TelegramUrl}/bot{_telegramConfig.Token}/setwebhook?url={_telegramConfig.WebhookUrl}";
        await _httpClient.GetAsync(setWebhookUrl, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}