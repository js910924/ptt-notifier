using infrastructure.Configs;
using Microsoft.Extensions.Options;

namespace api;

public class SetTelegramWebhookHostedService : IHostedService
{
    private readonly HttpClient _httpClient;
    private readonly TelegramConfig _telegramConfig;

    public SetTelegramWebhookHostedService(IHttpClientFactory httpClientFactory, IOptions<TelegramConfig> options)
    {
        var httpClient = httpClientFactory.CreateClient();
        var telegramConfig = options.Value;

        httpClient.BaseAddress = new Uri(telegramConfig.TelegramUrl);
        _httpClient = httpClient;
        _telegramConfig = telegramConfig;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var setWebhookEndpoint = $"/bot{_telegramConfig.Token}/setwebhook?url={_telegramConfig.WebhookUrl}";
        await _httpClient.GetAsync(setWebhookEndpoint, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}