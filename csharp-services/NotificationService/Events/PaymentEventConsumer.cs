using System.Text;
using System.Text.Json;
using NotificationService.Models;
using NotificationService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Events;

public class PaymentEventConsumer : BackgroundService
{
    private readonly RabbitMQService _rabbitMQService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<PaymentEventConsumer> _logger;
    private const string ExchangeName = "payment_events";
    private const string QueueName = "notification_payment_created";
    private const string RoutingKey = "payment.created";

    public PaymentEventConsumer(
        RabbitMQService rabbitMQService,
        INotificationService notificationService,
        ILogger<PaymentEventConsumer> logger)
    {
        _rabbitMQService = rabbitMQService;
        _notificationService = notificationService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var connection = _rabbitMQService.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
        channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(QueueName, ExchangeName, RoutingKey);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var paymentEvent = JsonSerializer.Deserialize<PaymentCreatedEvent>(message);
                await ProcessPaymentCreatedEvent(paymentEvent);
                channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment created event");
                channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessPaymentCreatedEvent(PaymentCreatedEvent paymentEvent)
    {
        var notificationMessage = $"Payment of {paymentEvent.Amount} {paymentEvent.Currency} " +
                                  $"for reservation {paymentEvent.ReservationId} has been processed.";
        await _notificationService.SendNotification(notificationMessage);
    }
}