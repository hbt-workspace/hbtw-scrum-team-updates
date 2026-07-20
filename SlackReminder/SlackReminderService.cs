using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SlackReminder;

public class SlackReminderService : ISlackReminderService
{
    private readonly ILogger<SlackReminderService> _logger;
    private readonly HttpClient _httpClient;
    private readonly SlackOptions _options;

    public SlackReminderService(
        IHttpClientFactory httpClientFactory,
        IOptions<SlackOptions> options,
        ILogger<SlackReminderService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendDailyReminderAsync(
        CancellationToken cancellationToken = default)
    {
        // TODO:
        // Get reminder data from database
        _logger.LogInformation("Sending daily Slack reminder...");

        await SendMessageAsync(cancellationToken);

        _logger.LogInformation("Daily Slack reminder completed.");
    }

    public async Task SendMessageAsync(CancellationToken cancellationToken = default)
    {
        // TODO:
        // Call Slack API
        var payload = new
        {
            channel = _options.ChannelId,
            text = _options.MessageTemplate
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.SlackAPIUrl);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.BotToken);

        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        // Throws if HTTP status is not 2xx
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        using var json = JsonDocument.Parse(responseBody);

        bool ok = json.RootElement.GetProperty("ok").GetBoolean();

        if (!ok)
        {
            string? error = json.RootElement.TryGetProperty("error", out var errorElement)
                ? errorElement.GetString()
                : "Unknown error";

            _logger.LogError(
                "Slack API returned an error. Channel: {Channel}, Error: {Error}",
                _options.ChannelId,
                error);

            throw new InvalidOperationException($"Slack API error: {error}");
        }

        _logger.LogInformation(
            "Slack message sent successfully to channel {Channel}.",
            _options.ChannelId);
    }
}