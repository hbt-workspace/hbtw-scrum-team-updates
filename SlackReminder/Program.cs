using SlackReminder;
using SlackReminder.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.Configure<SlackOptions>(
    builder.Configuration.GetSection("Slack"));

builder.Services.Configure<DailyScrumOptions>(
    builder.Configuration.GetSection("DailyScrum"));

builder.Services.AddHttpClient();
builder.Services.AddScoped<ISlackReminderService, SlackReminderService>();

// Add the daily worker service to the DI container
builder.Services.AddHostedService<DailyWorkerService>();

var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.UseHttpsRedirection();

// Security middleware
app.UseApiKeyAuthentication();

// Map the Slack channel reminder endpoint
app.MapSlackChannelReminderEndpoint();

app.Run();