namespace SlackReminderConsoleApp;

public class SlackOptions
{
    public string SlackAPIUrl { get; set; } = string.Empty;
    public string BotToken { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
    public string MessageTemplate { get; set; } = string.Empty;
}