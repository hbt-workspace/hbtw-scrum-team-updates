using Microsoft.Extensions.Hosting;

namespace SlackReminder;

public class DailyWorkerService : BackgroundService
{
    private readonly ILogger<DailyWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public DailyWorkerService(
        ILogger<DailyWorkerService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily Worker Service Started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime now = DateTime.Now;
            DateTime nextRun = GetNextRunTime(now);

            TimeSpan delay = nextRun - now;

            _logger.LogInformation(
                "Next execution at {NextRun}",
                nextRun);

            await Task.Delay(delay, stoppingToken);

            try
            {
                await DoWorkAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while executing daily job.");
            }
        }
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
            "Daily job started at {Time}",
            DateTime.Now);

            using IServiceScope scope = _serviceProvider.CreateScope();

            var slackReminderService =
                scope.ServiceProvider.GetRequiredService<ISlackReminderService>();

            await slackReminderService.SendDailyReminderAsync(cancellationToken);

            await Task.CompletedTask;

            _logger.LogInformation(
                "Daily job completed at {Time}",
                DateTime.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending daily reminder.");
        }

    }

    private static DateTime GetNextRunTime(DateTime now)
    {
        // Today's scheduled time (10:30 AM)
        DateTime nextRun = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            10, 30, 0);

        // If today's time has passed, move to tomorrow
        if (now >= nextRun)
        {
            nextRun = nextRun.AddDays(1);
        }

        // Skip Saturday and Sunday
        while (nextRun.DayOfWeek == DayOfWeek.Saturday ||
               nextRun.DayOfWeek == DayOfWeek.Sunday)
        {
            nextRun = nextRun.AddDays(1);
        }

        return nextRun;
    }
}