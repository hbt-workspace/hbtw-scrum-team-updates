namespace SlackReminder;

public interface ISlackReminderService
{
    Task SendDailyReminderAsync(CancellationToken cancellationToken = default);

    Task SendMessageAsync(CancellationToken cancellationToken = default);    
}