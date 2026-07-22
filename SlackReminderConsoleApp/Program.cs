using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SlackReminderConsoleApp;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<SlackOptions>(
            context.Configuration.GetSection("SlackOptions"));

        services.AddHttpClient();

        services.AddTransient<ISlackReminderService, SlackReminderService>();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
    })
    .Build();

using var scope = host.Services.CreateScope();

var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var service = scope.ServiceProvider.GetRequiredService<ISlackReminderService>();
// Only execute on Monday to Friday 
if (DateTime.Now.DayOfWeek is not DayOfWeek.Saturday
    && DateTime.Now.DayOfWeek is not DayOfWeek.Sunday)
{
    logger.LogInformation("Today is {Day}. Sending Slack reminder...", DateTime.Now.DayOfWeek);

    await service.SendDailyReminderAsync();

    logger.LogInformation("Slack reminder completed.");
}
else
{
    logger.LogInformation(
        "Today is {Day}. Slack reminder skipped because it only runs on Monday to Friday.",
        DateTime.Now.DayOfWeek);
}

Console.WriteLine("Daily job completed at {Time}.", DateTime.Now);