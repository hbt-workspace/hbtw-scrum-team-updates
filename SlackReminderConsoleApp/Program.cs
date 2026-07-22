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

var service = scope.ServiceProvider.GetRequiredService<ISlackReminderService>();

await service.SendDailyReminderAsync();

Console.WriteLine("Finished.");