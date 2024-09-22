namespace NotificationService.Services;

public interface INotificationService
{
    Task SendNotification(string message);
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendNotification(string message)
    {
        _logger.LogInformation($"Sending notification: {message}");
        // İşlemler Gerçek bir Notification işlklemi
        return Task.CompletedTask;
    }
}