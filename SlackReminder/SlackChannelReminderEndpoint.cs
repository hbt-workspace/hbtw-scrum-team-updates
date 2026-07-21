namespace SlackReminder;

public static class SlackChannelReminderEndpoint
{
    public static IEndpointRouteBuilder MapSlackChannelReminderEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/slack-reminder");

        group.MapGet("/send", async (ISlackReminderService slackReminderService,
            CancellationToken cancellationToken) =>
        {
            var today = DateTime.Now.DayOfWeek;

            if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
            {
                return Results.BadRequest(new
                {
                    Success = false,
                    Message = "Daily reminder cannot be sent on Saturdays or Sundays."
                });
            }

            await slackReminderService.SendDailyReminderAsync(cancellationToken);

            return Results.Ok(new
            {
                Success = true,
                Message = "Daily reminder sent successfully."
            });
        });       

        return group;
    }
}